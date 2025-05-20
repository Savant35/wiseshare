import { DashboardHeader } from "@/components/dashboard-header"
import { DashboardShell } from "@/components/dashboard-shell"
import { PaymentTable } from "@/components/payment-table"
//import { Button } from "@/components/ui/button"
//import { Input } from "@/components/ui/input"
//import { Search, Filter, Download } from "lucide-react"
/*
<div className="flex items-center gap-2">
          <div className="relative">
            <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input type="search" placeholder="Search payments..." className="w-[200px] pl-8 md:w-[300px]" />
          </div>
          <Button variant="outline">
            <Filter className="mr-2 h-4 w-4" />
            Filter
          </Button>
          <Button variant="outline">
            <Download className="mr-2 h-4 w-4" />
            Export
          </Button>
        </div>
        */
export default function PaymentsPage() {
  return (
    <DashboardShell>
      <DashboardHeader heading="Payment & Transactions" text="View and manage all payments on the platform">
        
      </DashboardHeader>
      <PaymentTable />
    </DashboardShell>
  )
}
