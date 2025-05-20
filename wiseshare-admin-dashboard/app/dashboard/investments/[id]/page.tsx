// app/dashboard/investments/[id]/page.tsx
import { getServerSession } from "next-auth/next"
import { redirect } from "next/navigation"
import { authOptions } from "@/lib/authOptions"
import Link from "next/link"
import { DashboardShell } from "@/components/dashboard-shell"
import { DashboardHeader } from "@/components/dashboard-header"
import { InvestmentDetails } from "@/components/investment-details"

export default async function InvestmentDetailsPage({
  params,
}: {
  params: Promise<{ id: string }>
}) {
  // await the params object before extracting id
  const { id } = await params

  const session = await getServerSession(authOptions)
  if (!session || session.user.role !== "Admin") {
    redirect("/login")
  }

  return (
    <DashboardShell>
      <DashboardHeader heading="Investment Details" text="Investment information">
        <Link href="/dashboard/investments">Back to Investments</Link>
      </DashboardHeader>
      <InvestmentDetails investmentId={id} />
    </DashboardShell>
  )
}
