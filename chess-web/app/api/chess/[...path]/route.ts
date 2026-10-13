import { NextRequest, NextResponse } from "next/server";

export const dynamic = "force-dynamic";

type RouteContext = { params: Promise<{ path: string[] }> };

async function forward(request: NextRequest, context: RouteContext) {
  const { path } = await context.params;
  const configuredBase = process.env.CHESS_API_BASE_URL?.replace(/\/$/, "");
  const bases = Array.from(new Set([
    configuredBase,
    "https://localhost:44336",
    "https://localhost:5001",
    "http://localhost:25068",
    "http://localhost:5000",
  ].filter((base): base is string => Boolean(base))));
  const endpointPath = `/chess/${path.map(encodeURIComponent).join("/")}${request.nextUrl.search}`;
  const body = request.method === "GET" || request.method === "HEAD" ? undefined : await request.text();
  const failures: string[] = [];

  for (const base of bases) {
    try {
      
      const response = await fetch(`${base}${endpointPath}`, {
        method: request.method,
        headers: request.headers.get("content-type") ? { "Content-Type": request.headers.get("content-type")! } : undefined,
        body,
        cache: "no-store",
      });
      const responseBody = await response.text();
      return new NextResponse(responseBody, {
        status: response.status,
        headers: { "Content-Type": response.headers.get("content-type") || "application/json" },
      });
    } catch (error) {
      failures.push(`${base}: ${error instanceof Error ? error.message : "connection failed"}`);
    }
  }

  return NextResponse.json({
    message: "Could not reach Chess.Api. Make sure the API is running and its local HTTPS certificate is trusted (dotnet dev-certs https --trust).",
    attempts: failures,
  }, { status: 502 });
}

export const GET = forward;
export const POST = forward;
