// src/lib/dropdown-options.ts

export interface DropdownOption {
  value: string;
  label: string;
}

export const genderOptions: DropdownOption[] = [
  { value: "male", label: "Male" },
  { value: "female", label: "Female" },
  { value: "other", label: "Other" },
  { value: "prefer_not_to_say", label: "Prefer not to say" },
];

export const countryOptions: DropdownOption[] = [
  { value: "usa", label: "United States" },
  { value: "canada", label: "Canada" },
  { value: "uk", label: "United Kingdom" },
  { value: "australia", label: "Australia" },
  { value: "germany", label: "Germany" },
  { value: "france", label: "France" },
  { value: "japan", label: "Japan" },
  { value: "singapore", label: "Singapore" },
  { value: "malaysia", label: "Malaysia" },
  { value: "indonesia", label: "Indonesia" },
  { value: "india", label: "India" },
  { value: "china", label: "China" },
  { value: "brazil", label: "Brazil" },
  { value: "mexico", label: "Mexico" },
  { value: "south_africa", label: "South Africa" },
  // Add more as needed
];

export const timezoneOptions: DropdownOption[] = [
  // Americas
  { value: "pst", label: "PST (Pacific Standard Time) UTC-8" },
  { value: "mst", label: "MST (Mountain Standard Time) UTC-7" },
  { value: "cst", label: "CST (Central Standard Time) UTC-6" },
  { value: "est", label: "EST (Eastern Standard Time) UTC-5" },
  { value: "ast", label: "AST (Atlantic Standard Time) UTC-4" },
  
  // Europe/Africa
  { value: "gmt", label: "GMT (Greenwich Mean Time) UTC+0" },
  { value: "cet", label: "CET (Central European Time) UTC+1" },
  { value: "eet", label: "EET (Eastern European Time) UTC+2" },
  { value: "cat", label: "CAT (Central Africa Time) UTC+2" },
  
  // Asia
  { value: "msk", label: "MSK (Moscow Time) UTC+3" },
  { value: "ist", label: "IST (Indian Standard Time) UTC+5:30" },
  { value: "wib", label: "WIB (Waktu Indonesia Barat) UTC+7" },
  { value: "wita", label: "WITA (Waktu Indonesia Tengah) UTC+8" },
  { value: "wit", label: "WIT (Waktu Indonesia Timur) UTC+9" },
  { value: "cst_china", label: "CST (China Standard Time) UTC+8" },
  { value: "jst", label: "JST (Japan Standard Time) UTC+9" },
  { value: "aest", label: "AEST (Australian Eastern Time) UTC+10" },
  
  // Other regions
  { value: "nzst", label: "NZST (New Zealand Standard Time) UTC+12" },
  { value: "sst", label: "SST (Samoa Standard Time) UTC-11" },
];

// Additional utility function to find option by value
export function findOptionByValue(options: DropdownOption[], value: string): DropdownOption | undefined {
  return options.find(option => option.value === value);
}

// Utility function to get all country values
export function getAllCountryValues(): string[] {
  return countryOptions.map(country => country.value);
}

// Utility function to get all timezone values
export function getAllTimezoneValues(): string[] {
  return timezoneOptions.map(tz => tz.value);
}