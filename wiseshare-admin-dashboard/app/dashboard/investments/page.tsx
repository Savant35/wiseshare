import { DashboardHeader } from "@/components/dashboard-header"
import { DashboardShell } from "@/components/dashboard-shell"
import { InvestmentTable } from "@/components/investment-table"
import { Button } from "@/components/ui/button"
//import { Input } from "@/components/ui/input"
import { Search, Filter } from "lucide-react"
/*
            <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input type="search" placeholder="Search investments..." className="w-[200px] pl-8 md:w-[300px]" />
            */
/*
           <Filter className="mr-2 h-4 w-4" />
           Filter
           */
/*
<Button variant="outline">
 
</Button>
*/
export default function InvestmentsPage() {
  return (
    <DashboardShell>
      <DashboardHeader heading="Investment Overview" text="View and manage all investments on the platform">
        <div className="flex items-center gap-2">
          <div className="relative">
          </div>

        </div>
      </DashboardHeader>
      <InvestmentTable />
    </DashboardShell>
  )
}
