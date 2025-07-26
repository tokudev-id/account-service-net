import React, { useMemo, useCallback, useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
// import EditProfileForm from '@/components/profile/edit-profile-form';
// import SetPasswordForm from '@/components/profile/set-password-form';
// import ChangePasswordForm from '@/components/profile/change-password-form';
// import ImageUploadModal from '@/components/profile/image-upload-modal';
import { Separator } from '@/components/ui/separator';

import { Mail, Edit3, LogOut, KeyRound, ShieldAlert, Pencil } from 'lucide-react';
import { Label } from '@/components/ui/label';
// import { useProfile } from '@/hooks/use-profile';
import {
  genderOptions,
  countryOptions,
  timezoneOptions,
  type DropdownOption,
} from '@lib/dropdown-options';
import { format, parseISO } from 'date-fns';
import { useToast } from '@/hooks/use-toast';
import { useProfile } from '@/hooks/use-profile';

interface ProfileFieldDisplayProps {
  label: string;
  value?: string;
}

const ProfileFieldDisplay: React.FC<ProfileFieldDisplayProps> = ({ label, value }) => (
  <div className="mb-4">
    <Label className="text-sm font-medium text-muted-foreground">{label}</Label>
    <div className="mt-1 p-3 h-10 w-full rounded-md border border-input bg-muted/50 text-sm flex items-center">
      {value || <span className="text-muted-foreground italic">Not set</span>}
    </div>
  </div>
);

const ProfilePage: React.FC = () => {
  const {
    userInfo,
    isLoading,
    error,
    isEditing,
    setIsEditing,
    handleLogout,
    fetchUserInfo,
  } = useProfile();
  const { toast } = useToast();

  const [showSetPasswordForm, setShowSetPasswordForm] = useState(false);
  const [showChangePasswordForm, setShowChangePasswordForm] = useState(false);
  const [isImageUploadModalOpen, setIsImageUploadModalOpen] = useState(false);
  const [pendingProfilePicture, setPendingProfilePicture] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);

  const findLabel = useCallback((options: DropdownOption[], value?: string): string | undefined => {
    if (!value) return undefined;
    const foundOption = options.find(opt => opt.value === value);
    return foundOption ? foundOption.label : value;
  }, []);

  const formatDisplayDate = useCallback((isoDateString?: string | null): string | undefined => {
    if (!isoDateString) return undefined;
    try {
      if (isoDateString.match(/^\d{4}-\d{2}-\d{2}$/)) {
        const date = parseISO(isoDateString);
        return format(date, "PPP");
      }
      return isoDateString;
    } catch (e) {
      console.warn("Error formatting date string:", isoDateString, e);
      return isoDateString;
    }
  }, []);

  const memoizedInitialData = useMemo(() => {
    if (!userInfo) return undefined;
    return {
      name: userInfo.name || '',
      gender: userInfo.gender || '',
      country: userInfo.country || '',
      birthdate: userInfo.birthdate || '',
      timezone: userInfo.timezone || '',
      email: userInfo.email || '',
    };
  }, [userInfo]);

  const handleCancelEdit = useCallback(() => {
    setIsEditing(false);
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    setPendingProfilePicture(null);
    setPreviewUrl(null);
  }, [setIsEditing, previewUrl]);

  const handleEditProfileSuccess = useCallback(() => {
    setIsEditing(false);
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    setPendingProfilePicture(null);
    setPreviewUrl(null);
    fetchUserInfo();
  }, [setIsEditing, fetchUserInfo, previewUrl]);

  const handleSetPasswordSuccess = () => {
    setShowSetPasswordForm(false);
    toast({ title: "Password Set", description: "Your password has been set. You will now be logged out." });
    handleLogout();
  };

  const handleChangePasswordSuccess = () => {
    setShowChangePasswordForm(false);
    toast({ title: "Password Changed", description: "Your password has been changed. You will now be logged out." });
    handleLogout();
  };

  const handleCropConfirm = (croppedFile: File) => {
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    setPendingProfilePicture(croppedFile);
    setPreviewUrl(URL.createObjectURL(croppedFile));
    setIsImageUploadModalOpen(false);
  };

  useEffect(() => {
    const currentPreview = previewUrl;
    return () => {
      if (currentPreview) URL.revokeObjectURL(currentPreview);
    };
  }, [previewUrl]);

  const isFormActive = isEditing || showSetPasswordForm || showChangePasswordForm;
  const currentAvatarSrc = previewUrl || userInfo?.picture || `https://placehold.co/100x100.png?text=${(userInfo?.name || 'U').charAt(0)}`;

  if (isLoading && !userInfo) {
    return (
      <div className="container mx-auto p-4 md:p-8">
        <Skeleton className="h-32 w-full mb-6 rounded-lg" />
        <Card className="w-full">
          <CardContent className="pt-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {[...Array(6)].map((_, i) => (
                <Skeleton key={i} className="h-16 w-full" />
              ))}
            </div>
            <Separator className="my-6" />
            <Skeleton className="h-10 w-32" />
            <Skeleton className="h-10 w-24 mt-4" />
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-secondary/50">
      <div className="bg-gradient-to-r from-primary/10 via-primary/5 to-secondary/20 shadow-sm p-8">
        <div className="container mx-auto">
          {userInfo && (
            <div className="flex flex-col sm:flex-row items-center justify-between">
              <div className="flex items-center space-x-4 mb-4 sm:mb-0">
                <div className="relative group">
                  <img src={currentAvatarSrc} alt={userInfo.name || 'User Avatar'} width={80} height={80} className="rounded-full border-2 border-background shadow-md object-cover" />
                  {isEditing && (
                    <Button variant="outline" size="icon" className="absolute bottom-0 right-0 h-8 w-8 rounded-full opacity-0 group-hover:opacity-100 transition-opacity duration-200 bg-background/70 hover:bg-background" onClick={() => setIsImageUploadModalOpen(true)} title="Change Profile Picture">
                      <Pencil className="h-4 w-4" />
                    </Button>
                  )}
                </div>
                <div>
                  <h1 className="text-2xl font-semibold text-foreground">{userInfo.name || 'User Name'}</h1>
                  <p className="text-sm text-muted-foreground">{userInfo.email}</p>
                </div>
              </div>
              {/* {!isFormActive && (
                <Button onClick={() => setIsEditing(true)} size="sm" disabled={isFormActive}>
                  <Edit3 className="mr-2 h-4 w-4" /> Edit Profile
                </Button>
              )} */}
            </div>
          )}
        </div>
      </div>

      <div className="container mx-auto p-4 md:p-8">
        <Card className="w-full -mt-12 sm:-mt-14 relative z-10 shadow-lg">
                  <CardContent className="pt-10">
            {error && <p className="text-destructive text-sm mb-4">Error loading profile: {error}</p>}

            {isEditing && userInfo && memoizedInitialData ? (
                <></>
            //   <EditProfileForm
            //     initialData={memoizedInitialData}
            //     onSaveSuccess={handleEditProfileSuccess}
            //     onCancel={handleCancelEdit}
            //     pendingProfilePicture={pendingProfilePicture}
            //   />
            ) : showSetPasswordForm ? (
                <></>
            //   <SetPasswordForm onSuccess={handleSetPasswordSuccess} onCancel={() => setShowSetPasswordForm(false)} />
            ) : showChangePasswordForm ? (
                <></>
            //   <ChangePasswordForm onSuccess={handleChangePasswordSuccess} onCancel={() => setShowChangePasswordForm(false)} />
            ) : userInfo ? (
              <div className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-x-8 gap-y-2">
                  <ProfileFieldDisplay label="Full Name" value={userInfo.name} />
                  <ProfileFieldDisplay label="Gender" value={findLabel(genderOptions, userInfo.gender)} />
                  <ProfileFieldDisplay label="Country" value={findLabel(countryOptions, userInfo.country)} />
                  <ProfileFieldDisplay label="Birthday" value={formatDisplayDate(userInfo.birthdate)} />
                  <ProfileFieldDisplay label="Time Zone" value={findLabel(timezoneOptions, userInfo.timezone)} />
                </div>
                <Separator className="my-8" />
                <div>
                  <h3 className="text-lg font-semibold mb-4 text-foreground">Account Security</h3>
                  <div className="space-y-4">
                    <div className="flex items-center space-x-3 p-4 border rounded-lg bg-muted/20">
                      <div className="p-2 bg-primary/10 rounded-full">
                        <Mail className="h-5 w-5 text-primary" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-foreground">{userInfo.email}</p>
                        <p className="text-xs text-muted-foreground">{userInfo.isEmailVerified ? "Verified" : "Not Verified"}</p>
                      </div>
                    </div>
                    {!userInfo.has_password && (
                      <div>
                        <Button variant="outline" onClick={() => setShowSetPasswordForm(true)} disabled={isFormActive}>
                          <KeyRound className="mr-2 h-4 w-4" /> Set Password
                        </Button>
                        <p className="text-xs text-muted-foreground mt-1">Allows you to log in with a password in addition to any third-party providers.</p>
                      </div>
                    )}
                    {/* {userInfo.has_password && (
                      <div>
                        <Button variant="outline" onClick={() => setShowChangePasswordForm(true)} disabled={isFormActive}>
                          <ShieldAlert className="mr-2 h-4 w-4" /> Change Password
                        </Button>
                        {userInfo.latest_password_changed_at && (
                          <p className="text-xs text-muted-foreground mt-1">Password last changed: {formatDisplayDate(userInfo.latest_password_changed_at)}</p>
                        )}
                      </div>
                    )} */}
                  </div>
                </div>
              </div>
            ) : (
              !isLoading && <p className="text-sm text-muted-foreground">Could not load user data.</p>
            )}

            {!isFormActive && (
              <div className="mt-8 border-t pt-6 flex justify-start">
                <Button onClick={handleLogout} variant="outline" disabled={isLoading}>
                  <LogOut className="mr-2 h-4 w-4" /> {isLoading ? 'Processing...' : 'Logout'}
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      {/* {userInfo && (
        <ImageUploadModal
          isOpen={isImageUploadModalOpen}
          onClose={() => setIsImageUploadModalOpen(false)}
          onCropConfirm={handleCropConfirm}
        />
      )} */}
    </div>
  );
};

export default ProfilePage;
