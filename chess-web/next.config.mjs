/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  distDir: "next-build",
  experimental: {
    cpus: 1,
    workerThreads: false,
  },
};

export default nextConfig;
