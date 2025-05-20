import { DashboardHeader } from "@/components/dashboard-header"
import { DashboardShell } from "@/components/dashboard-shell"
import { UserTable } from "@/components/user-table"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Search, UserPlus } from "lucide-react"
import { AddUserDialog } from "@/components/add-user-dialog"
//<Input type="search" placeholder="Search users..." className="w-[200px] pl-8 md:w-[300px]" />
//<Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />

export default function UsersPage() {
  return (
    <DashboardShell>
      <DashboardHeader heading="User Management" text="View and manage all users on the platform">
        <div className="flex items-center gap-2">
          <div className="relative">
          </div>
          <AddUserDialog>
            <Button>
              <UserPlus className="mr-2 h-4 w-4" />
              Add User
            </Button>
          </AddUserDialog>
        </div>
      </DashboardHeader>
      <UserTable />
    </DashboardShell>
  )
}
