'use client';

import { useState, useEffect, useCallback, useMemo } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { format, parseISO, isValid } from 'date-fns';
import { useToast } from '@/hooks/use-toast';
import {
  genderOptions,
  countryOptions,
  timezoneOptions,
} from '@/lib/dropdown-options';

const profileFormSchema = z.object({
  name: z.string()
    .min(1, 'Name is required')
    .max(100, 'Name cannot exceed 100 characters'),
  gender: z.string().optional(),
  country: z.string().optional(),
  birthdate: z.date().nullable().optional(),
  timezone: z.string().optional(),
});

export type ProfileFormValues = z.infer<typeof profileFormSchema>;

interface UseEditProfileFormProps {
  initialData: {
    name: string;
    gender?: string;
    country?: string;
    birthdate?: string; // ISO format (YYYY-MM-DD)
    timezone?: string;
  };
  onSaveSuccess: () => void;
}

export function useEditProfileForm({ initialData, onSaveSuccess }: UseEditProfileFormProps) {
  const { toast } = useToast();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const defaultValues = useMemo(() => {
    let parsedBirthdate = null;
    if (initialData.birthdate) {
      const date = parseISO(initialData.birthdate);
      if (isValid(date)) {
        parsedBirthdate = date;
      } else {
        console.warn("Invalid birthdate string received in initialData:", initialData.birthdate);
      }
    }
    return {
      name: initialData.name || '',
      gender: initialData.gender || '',
      country: initialData.country || '',
      birthdate: parsedBirthdate,
      timezone: initialData.timezone || '',
    };
  }, [
    initialData.name,
    initialData.gender,
    initialData.country,
    initialData.birthdate,
    initialData.timezone
  ]);

  const form = useForm<ProfileFormValues>({
    resolver: zodResolver(profileFormSchema),
    defaultValues,
    mode: 'onBlur',
  });

  const resetForm = useCallback(() => {
    form.reset(defaultValues);
  }, [form, defaultValues]);

  useEffect(() => {
    resetForm();
  }, [resetForm]);

  const handleSubmit = useCallback(async (values: ProfileFormValues, pendingImageFile: File | null) => {
    setIsSubmitting(true);
    try {
      // Step 1: Upload profile picture if provided
      if (pendingImageFile) {
        const formData = new FormData();
        formData.append('profilePicture', pendingImageFile);

        const imageUploadResponse = await fetch(`/api/profile/picture`, {
          method: 'POST',
          body: formData,
          credentials: 'include', // send cookies for auth
        });

        if (!imageUploadResponse.ok) {
          const errorData = await imageUploadResponse.json().catch(() => ({ message: 'Failed to upload profile picture.' }));
          throw new Error(errorData.message || 'Profile picture upload failed.');
        }
        toast({
          title: "Profile Picture Updated",
          description: "Your new profile picture has been uploaded.",
        });
      }

      // Step 2: Patch profile info
      const profileDataPayload = {
        name: values.name,
        gender: values.gender || "",
        country: values.country || "",
        timezone: values.timezone || "",
        birthdate: values.birthdate && isValid(values.birthdate)
          ? format(values.birthdate, 'yyyy-MM-dd')
          : "",
      };

      const profileUpdateResponse = await fetch(`/api/profile`, {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
        },
        credentials: 'include', // needed for cookie-based auth
        body: JSON.stringify(profileDataPayload),
      });

      if (!profileUpdateResponse.ok) {
        const errorData = await profileUpdateResponse.json().catch(() => ({ message: 'Failed to update profile information.' }));
        throw new Error(errorData.message || 'Profile information update failed.');
      }

      toast({
        title: "Profile Saved",
        description: "Your profile changes have been saved successfully.",
      });
      onSaveSuccess(); 
    } catch (error) {
      console.error('Profile save error:', error);
      toast({
        title: "Save Failed",
        description: error instanceof Error 
          ? error.message 
          : 'An unexpected error occurred. Please try again.',
        variant: "destructive",
      });
    } finally {
      setIsSubmitting(false);
    }
  }, [toast, onSaveSuccess]);

  const dropdownOptions = useMemo(() => ({
    gender: genderOptions,
    country: countryOptions,
    timezone: timezoneOptions,
  }), []);

  return {
    form,
    handleSubmit,
    isSubmitting,
    dropdownOptions,
  };
}
