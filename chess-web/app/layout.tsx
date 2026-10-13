import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Chessroom — Play chess",
  description: "A clean chess board powered by the ChessGame API.",
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
