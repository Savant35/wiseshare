import { DashboardHeader } from "@/components/dashboard-header"
import { DashboardShell } from "@/components/dashboard-shell"
import { PendingApprovalsTable } from "@/components/pending-approvals-table"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Search, Filter } from "lucide-react"

export default function PendingApprovalsPage() {
  return (
    <DashboardShell>
      <DashboardHeader heading="Pending Approvals" text="View and manage all pending sell requests">
        <div className="flex items-center gap-2">
          <div className="relative">
            <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input type="search" placeholder="Search requests..." className="w-[200px] pl-8 md:w-[300px]" />
          </div>
          <Button variant="outline">
            <Filter className="mr-2 h-4 w-4" />
            Filter
          </Button>
        </div>
      </DashboardHeader>
      <PendingApprovalsTable />
    </DashboardShell>
  )
}
