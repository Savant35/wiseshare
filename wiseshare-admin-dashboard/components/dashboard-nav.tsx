"use client"

import type React from "react"

import Link from "next/link"
import { usePathname } from "next/navigation"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { LayoutDashboard, Users, Home, TrendingUp, CreditCard, Settings, LogOut, CheckCircle } from "lucide-react"
import { signOut } from "next-auth/react"

interface DashboardNavProps extends React.HTMLAttributes<HTMLElement> { }

export function DashboardNav({ className, ...props }: DashboardNavProps) {
  const pathname = usePathname()

  const navItems = [
    {
      title: "Dashboard",
      href: "/dashboard",
      icon: LayoutDashboard,
    },
    {
      title: "Users",
      href: "/dashboard/users",
      icon: Users,
    },
    {
      title: "Properties",
      href: "/dashboard/properties",
      icon: Home,
    },
    {
      title: "Investments",
      href: "/dashboard/investments",
      icon: TrendingUp,
    },
    {
      title: "Payments",
      href: "/dashboard/payments",
      icon: CreditCard,
    },
    {
      title: "Pending Approvals",
      href: "/dashboard/pending-approvals",
      icon: CheckCircle,
    },
    /*
    {
      title: "Settings",
      href: "/dashboard/settings",
      icon: Settings,
    },
    */
  ]

  return (
    <nav className={cn("flex flex-col space-y-1 p-4", className)} {...props}>
      <div className="flex h-12 items-center px-4 mb-4">
        <Link href="/dashboard" className="flex items-center gap-2 font-bold text-xl">
          <span className="h-7 w-7 rounded-full bg-primary flex items-center justify-center text-primary-foreground">
            W
          </span>
          WiseShare
        </Link>
      </div>
      {navItems.map((item) => (
        <Button
          key={item.href}
          variant={pathname === item.href ? "secondary" : "ghost"}
          className={cn(
            "w-full justify-start",
            pathname === item.href ? "bg-secondary" : "hover:bg-transparent hover:underline",
          )}
          asChild
        >
          <Link href={item.href}>
            <item.icon className="mr-2 h-4 w-4" />
            {item.title}
          </Link>
        </Button>
      ))}
      <div className="mt-auto pt-4">
        <Button variant="ghost" className="w-full justify-start text-muted-foreground"
          onClick={() => signOut({ callbackUrl: "/login" })}> <LogOut className="mr-2 h-4 w-4" />
          Logout
        </Button>
      </div>
    </nav>
  )
}
