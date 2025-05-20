// lib/authOptions.ts
import { NextAuthOptions } from "next-auth"
import CredentialsProvider from "next-auth/providers/credentials"

export const authOptions: NextAuthOptions = {
  providers: [
    CredentialsProvider({
      name: "Admin Login",
      credentials: {
        email: { label: "Email", type: "email" },
        password: { label: "Password", type: "password" },
      },
      async authorize(credentials) {
        if (!credentials) return null

        const res = await fetch(`${process.env.API_URL}/auth/login`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(credentials),
        })
        const data = await res.json()

        if (!res.ok) {
          const msg =
            (Array.isArray(data.Errors) && data.Errors[0]) ||
            data.Message ||
            "Invalid email or password."
          throw new Error(msg)
        }
        if (data.role !== "Admin") {
          throw new Error("You do not have permission to access this page.")
        }

        return {
          id: data.userId,
          name: `${data.firstName} ${data.lastName}`,
          email: credentials.email,
          role: data.role,
          accessToken: data.token,
        }
      },
    }),
  ],
  session: { strategy: "jwt" },
  pages: { signIn: "/login" },
  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        const u = user as any
        token.role = u.role
        token.accessToken = u.accessToken
      }
      return token
    },
    async session({ session, token }) {
      const u = session.user as any
      u.id = token.sub!
      u.role = token.role
      u.accessToken = token.accessToken
      return session
    },
  },
  secret: process.env.NEXTAUTH_SECRET,
}
