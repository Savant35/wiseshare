// app/dashboard/users/[id]/page.tsx
import { getServerSession } from "next-auth/next"
import { redirect } from "next/navigation"
import { authOptions } from "@/lib/authOptions"
import Link from "next/link"
import { DashboardShell } from "@/components/dashboard-shell"
import { DashboardHeader } from "@/components/dashboard-header"
import { UserProfile } from "@/components/user-profile"

export default async function UserDetailsPage({params,}: { params: { id: string }}) {
  const session = await getServerSession(authOptions)
  if (!session || session.user.role !== "Admin") {
    redirect("/login")
  }
  return (
    <DashboardShell>
      <DashboardHeader heading="User Details" text="User information">
        <Link href="/dashboard/users">Back to Users</Link>
      </DashboardHeader>
      <UserProfile userId={params.id} />
    </DashboardShell>
  )
}
