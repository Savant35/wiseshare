// app/dashboard/properties/[id]/page.tsx
import { getServerSession } from "next-auth/next"
import { redirect } from "next/navigation"
import { authOptions } from "@/lib/authOptions"
import Link from "next/link"
import { DashboardShell } from "@/components/dashboard-shell"
import { DashboardHeader } from "@/components/dashboard-header"
import { PropertyDetails } from "@/components/property-details"

export default async function PropertyDetailsPage({
  params,
}: {
  // params is now a Promise, per Next.js dynamic API requirements
  params: Promise<{ id: string }>
}) {
  // await params before using it
  const { id } = await params

  const session = await getServerSession(authOptions)
  if (!session || session.user.role !== "Admin") {
    redirect("/login")
  }

  return (
    <DashboardShell>
      <DashboardHeader heading="Property Details" text="Property information">
        <Link href="/dashboard/properties">Back to Properties</Link>
      </DashboardHeader>
      <PropertyDetails propertyId={id} />
    </DashboardShell>
  )
}
