/* eslint-disable @typescript-eslint/no-explicit-any */
// src/hooks/use-change-pin-form.ts
import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

const formSchema = z
  .object({
    password: z.string().min(6, "Password is required"),
    newPin: z.string().min(4).max(6, "PIN must be 4-6 digits"),
    confirmPin: z.string().min(4).max(6, "Confirm PIN must be 4-6 digits"),
  })
  .refine((data) => data.newPin === data.confirmPin, {
    message: "PINs do not match",
    path: ["confirmPin"],
  });

type FormValues = z.infer<typeof formSchema>;

export function useChangePinForm() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [statusMessage, setStatusMessage] = useState<string | null>(null);
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
  });

  const onSubmit = async (values: FormValues) => {
    setIsSubmitting(true);
    setStatusMessage(null);

    try {
      const res = await fetch("/api/profile/change-pin", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          password: values.password,
          newPin: values.newPin,
        }),
      });

      if (!res.ok) {
        const err = await res.json();
        throw new Error(err.message || "Failed to change PIN");
      }

      setStatusMessage("PIN updated successfully!");
      form.reset();
    } catch (error: any) {
      form.setError("root", { message: error.message });
    } finally {
      setIsSubmitting(false);
    }
  };

  return { form, onSubmit, isSubmitting, statusMessage };
}
