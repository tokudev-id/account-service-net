// src/components/profile/edit-profile-form.tsx
'use client';

import * as React from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label'; // Not strictly needed if using FormLabel everywhere
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from "@/components/ui/command";
import { Calendar as CalendarIcon, Loader2, Save, X, Check, ChevronsUpDown } from 'lucide-react';
import { cn } from "@/lib/utils";
import { format } from "date-fns";
import { ScrollArea } from '@/components/ui/scroll-area';
import { useEditProfileForm, type ProfileFormValues } from '@/hooks/use-edit-profile-form';

interface EditProfileFormProps {
  initialData: {
    name: string;
    email: string; 
    gender?: string;
    country?: string;
    birthdate?: string; 
    timezone?: string;
  };
  pendingProfilePicture: File | null; // New prop
  onSaveSuccess: () => void;
  onCancel: () => void;
}

export default function EditProfileForm({ initialData, pendingProfilePicture, onSaveSuccess, onCancel }: EditProfileFormProps) {
  const {
    form,
    handleSubmit: handleFormSubmit, // Renamed to avoid conflict with internal handleSubmit
    isSubmitting,
    dropdownOptions,
  } = useEditProfileForm({
    initialData: {
      name: initialData.name,
      gender: initialData.gender,
      country: initialData.country,
      birthdate: initialData.birthdate,
      timezone: initialData.timezone,
    },
    onSaveSuccess,
  });

  const [genderOpen, setGenderOpen] = React.useState(false);
  const [countryOpen, setCountryOpen] = React.useState(false);
  const [timezoneOpen, setTimezoneOpen] = React.useState(false);

  // Wrapper for form submission to include pendingProfilePicture
  const onSubmit = (formValues: ProfileFormValues) => {
    handleFormSubmit(formValues, pendingProfilePicture);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-4">
          <FormField
            control={form.control}
            name="name"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Full Name</FormLabel>
                <FormControl>
                  <Input placeholder="Your full name" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="gender"
            render={({ field }) => (
              <FormItem className="flex flex-col">
                <FormLabel>Gender</FormLabel>
                <Popover open={genderOpen} onOpenChange={setGenderOpen}>
                  <PopoverTrigger asChild>
                    <FormControl>
                      <Button
                        variant="outline"
                        role="combobox"
                        aria-expanded={genderOpen}
                        className={cn(
                          "w-full justify-between",
                          !field.value && "text-muted-foreground"
                        )}
                      >
                        {field.value
                          ? dropdownOptions.gender.find(
                              (option) => option.value === field.value
                            )?.label
                          : "Select gender"}
                        <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                      </Button>
                    </FormControl>
                  </PopoverTrigger>
                  <PopoverContent className="w-[--radix-popover-trigger-width] p-0">
                    <Command>
                      <CommandInput placeholder="Search gender..." />
                      <CommandEmpty>No gender found.</CommandEmpty>
                      <CommandList>
                        <CommandGroup>
                          <ScrollArea className="max-h-48">
                            {dropdownOptions.gender.map((option) => (
                              <CommandItem
                                value={option.label}
                                key={option.value}
                                onSelect={() => {
                                  form.setValue("gender", option.value);
                                  setGenderOpen(false);
                                }}
                              >
                                <Check
                                  className={cn(
                                    "mr-2 h-4 w-4",
                                    option.value === field.value
                                      ? "opacity-100"
                                      : "opacity-0"
                                  )}
                                />
                                {option.label}
                              </CommandItem>
                            ))}
                          </ScrollArea>
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="country"
            render={({ field }) => (
              <FormItem className="flex flex-col">
                <FormLabel>Country</FormLabel>
                <Popover open={countryOpen} onOpenChange={setCountryOpen}>
                  <PopoverTrigger asChild>
                    <FormControl>
                      <Button
                        variant="outline"
                        role="combobox"
                        aria-expanded={countryOpen}
                        className={cn(
                          "w-full justify-between",
                          !field.value && "text-muted-foreground"
                        )}
                      >
                        {field.value
                          ? dropdownOptions.country.find(
                              (option) => option.value === field.value
                            )?.label
                          : "Select country"}
                        <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                      </Button>
                    </FormControl>
                  </PopoverTrigger>
                  <PopoverContent className="w-[--radix-popover-trigger-width] p-0">
                    <Command>
                      <CommandInput placeholder="Search country..." />
                      <CommandEmpty>No country found.</CommandEmpty>
                      <CommandList>
                        <CommandGroup>
                          <ScrollArea className="max-h-60">
                            {dropdownOptions.country.map((option) => (
                              <CommandItem
                                value={option.label}
                                key={option.value}
                                onSelect={() => {
                                  form.setValue("country", option.value);
                                  setCountryOpen(false);
                                }}
                              >
                                <Check
                                  className={cn(
                                    "mr-2 h-4 w-4",
                                    option.value === field.value
                                      ? "opacity-100"
                                      : "opacity-0"
                                  )}
                                />
                                {option.label}
                              </CommandItem>
                            ))}
                          </ScrollArea>
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="timezone"
            render={({ field }) => (
              <FormItem className="flex flex-col">
                <FormLabel>Time Zone</FormLabel>
                <Popover open={timezoneOpen} onOpenChange={setTimezoneOpen}>
                  <PopoverTrigger asChild>
                    <FormControl>
                      <Button
                        variant="outline"
                        role="combobox"
                        aria-expanded={timezoneOpen}
                        className={cn(
                          "w-full justify-between",
                          !field.value && "text-muted-foreground"
                        )}
                      >
                        {field.value
                          ? dropdownOptions.timezone.find(
                              (option) => option.value === field.value
                            )?.label
                          : "Select time zone"}
                        <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                      </Button>
                    </FormControl>
                  </PopoverTrigger>
                  <PopoverContent className="w-[--radix-popover-trigger-width] p-0">
                    <Command>
                      <CommandInput placeholder="Search time zone..." />
                      <CommandEmpty>No time zone found.</CommandEmpty>
                      <CommandList>
                         <CommandGroup>
                          <ScrollArea className="max-h-60"> 
                            {dropdownOptions.timezone.map((option) => (
                              <CommandItem
                                value={option.label}
                                key={option.value}
                                onSelect={() => {
                                  form.setValue("timezone", option.value);
                                  setTimezoneOpen(false);
                                }}
                              >
                                <Check
                                  className={cn(
                                    "mr-2 h-4 w-4",
                                    option.value === field.value
                                      ? "opacity-100"
                                      : "opacity-0"
                                  )}
                                />
                                {option.label}
                              </CommandItem>
                            ))}
                          </ScrollArea>
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <div className="space-y-2 mt-6">
          <Label htmlFor="email-display">Email Address</Label>
          <Input id="email-display" value={initialData.email} readOnly disabled className="bg-muted/50 cursor-not-allowed" />
          <p className="text-xs text-muted-foreground">Email address cannot be changed here.</p>
        </div>

        <div className="flex justify-end space-x-3 pt-6 border-t">
          <Button type="button" variant="outline" onClick={onCancel} disabled={isSubmitting}>
            <X className="mr-2 h-4 w-4" /> Cancel
          </Button>
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Save className="mr-2 h-4 w-4" />}
            Save Changes
          </Button>
        </div>
      </form>
    </Form>
  );
}
