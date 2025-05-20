import { DashboardHeader } from "@/components/dashboard-header"
import { DashboardShell } from "@/components/dashboard-shell"
import { PropertyTable } from "@/components/property-table"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Search, Plus } from "lucide-react"
import { AddPropertyDialog } from "@/components/add-property-dialog"
 /*
            <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input type="search" placeholder="Search properties..." className="w-[200px] pl-8 md:w-[300px]" />
            */

export default function PropertiesPage() {
  return (
    <DashboardShell>
      <DashboardHeader heading="Property Management" text="View and manage all properties on the platform">
        <div className="flex items-center gap-2">
          <div className="relative">
           
          </div>
          <AddPropertyDialog>
            <Button>
              <Plus className="mr-2 h-4 w-4" />
              Add Property
            </Button>
          </AddPropertyDialog>
        </div>
      </DashboardHeader>
      <PropertyTable />
    </DashboardShell>
  )
}
