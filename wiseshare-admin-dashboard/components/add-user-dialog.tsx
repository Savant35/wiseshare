"use client"

import type React from "react"

import { useState } from "react"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { registerUser } from "@/lib/api"

export function AddUserDialog({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = useState(false)
  const [processing, setProcessing] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    setError(null)
    setProcessing(true)

    try {
      const formData = new FormData(e.currentTarget)
      const userData = {
        firstName: formData.get("firstName") as string,
        lastName: formData.get("lastName") as string,
        email: formData.get("email") as string,
        phone: formData.get("phone") as string,
        //role: formData.get("role") as string,
        password: formData.get("password") as string,
        securityQuestion: formData.get("securityQuestion") as string,
        securityAnswer: formData.get("securityAnswer") as string,
      }

      await registerUser(userData)
      setOpen(false)
      // Refresh the page to show the new user
      window.location.reload()
    } catch (err: any) {
      console.error("Failed to create user:", err)
      setError(err.message || "Failed to create user")
    } finally {
      setProcessing(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>{children}</DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Add New User</DialogTitle>
          <DialogDescription>Create a new user account. All fields are required.</DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit}>
          <div className="grid gap-4 py-4">
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="firstName" className="text-right">
                First Name
              </Label>
              <Input id="firstName" name="firstName" className="col-span-3" required />
            </div>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="lastName" className="text-right">
                Last Name
              </Label>
              <Input id="lastName" name="lastName" className="col-span-3" required />
            </div>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="email" className="text-right">
                Email
              </Label>
              <Input id="email" name="email" type="email" className="col-span-3" required />
            </div>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="phone" className="text-right">
                Phone
              </Label>
              <Input id="phone" name="phone" className="col-span-3" required />
            </div>


            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="password" className="text-right">
                Password
              </Label>
              <Input id="password" name="password" type="password" className="col-span-3" required />
            </div>

            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="securityQuestion" className="text-right">
                Security Question
              </Label>
              <Input id="securityQuestion" name="securityQuestion" className="col-span-3" required/>
            </div>

            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="securityAnswer" className="text-right">
                Security Answer
              </Label>
              <Input id="securityAnswer" name="securityAnswer" className="col-span-3" required/>
            </div>

            {error && <p className="text-sm text-destructive text-center">{error}</p>}
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setOpen(false)} disabled={processing}>
              Cancel
            </Button>
            <Button type="submit" disabled={processing}>
              {processing ? "Creating..." : "Create User"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
