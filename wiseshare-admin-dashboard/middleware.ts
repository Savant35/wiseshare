import { NextResponse } from "next/server"
import { getToken } from "next-auth/jwt"
import { NextRequest } from "next/server"

export async function middleware(req: NextRequest) {
  if (req.nextUrl.pathname.startsWith("/dashboard")) {
    const token = await getToken({ req, secret: process.env.NEXTAUTH_SECRET })
    if (!token || token.role !== "Admin") {
      const url = req.nextUrl.clone()
      url.pathname = "/login"
      url.searchParams.set("callbackUrl", req.nextUrl.pathname)
      return NextResponse.redirect(url)
    }
  }
  return NextResponse.next()
}

export const config = { matcher: ["/dashboard/:path*"] }
