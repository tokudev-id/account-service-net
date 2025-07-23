import { useState, useEffect, useCallback, useRef } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { useToast } from './use-toast';
import { useNavigate, useLocation } from 'react-router-dom';
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { CheckCircle, XCircle } from 'lucide-react';
import ReCAPTCHA from 'react-google-recaptcha';

const loginSchema = z.object({
  email: z.string().email({ message: "Please enter a valid email address." }),
  password: z.string().min(1, { message: "Password is required." }),
});

type LoginFormValues = z.infer<typeof loginSchema>;

interface StatusAlertProps {
  type: 'success' | 'error' | 'info';
  title: string;
  description: string;
}

export function useLoginForm() {
  const { toast } = useToast();
  const navigate = useNavigate();
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);

  const [showPassword, setShowPassword] = useState(false);
  const [statusAlert, setStatusAlert] = useState<StatusAlertProps | null>(null);
  const [showResendLinkForEmail, setShowResendLinkForEmail] = useState<string | null>(null);
  const [isResendingConfirmation, setIsResendingConfirmation] = useState(false);

  const recaptchaRef = useRef<ReCAPTCHA>(null);
  const [isCaptchaVerified, setIsCaptchaVerified] = useState(false);
  const [captchaToken, setCaptchaToken] = useState('');

  const returnUrl = searchParams.get('returnUrl') ?? '';

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: '',
      password: '',
    },
  });

  const handleInitialStatusMessages = useCallback(() => {
    const confirmationStatus = searchParams.get('confirmed');
    const resetStatus = searchParams.get('reset');
    const errorStatus = searchParams.get('error');
    let alertToShow: StatusAlertProps | null = null;

    if (confirmationStatus === 'true') {
      alertToShow = {
        type: 'success',
        title: "Email Confirmed",
        description: "Your email address has been successfully confirmed. You can now login.",
      };
    } else if (resetStatus === 'success') {
      alertToShow = {
        type: 'success',
        title: "Password Reset Successful",
        description: "Your password has been reset. You can now login with your new password.",
      };
    } else if (errorStatus) {
      let title = "An Error Occurred";
      let description = "Something went wrong. Please try again later.";

      switch (errorStatus) {
        case 'confirmation_failed':
        case 'confirmation_invalid':
          title = "Email Confirmation Failed";
          description = "The confirmation link is invalid or has been used.";
          break;
        case 'confirmation_expired':
          title = "Email Confirmation Failed";
          description = "The confirmation link has expired. Please request a new one.";
          break;
        case 'reset_failed':
        case 'reset_invalid':
          title = "Password Reset Failed";
          description = "The password reset link is invalid or has been used.";
          break;
        case 'reset_expired':
          title = "Password Reset Failed";
          description = "The password reset link has expired. Please request a new one.";
          break;
        case 'login_required':
          title = "Login Required";
          description = "You need to be logged in to access this page.";
          break;
      }

      alertToShow = { type: 'error', title, description };
    }

    if (alertToShow) {
      setStatusAlert(alertToShow);

      // Clean URL (remove query params)
      const newSearch = returnUrl ? `?returnUrl=${encodeURIComponent(returnUrl)}` : '';
      navigate(`/login${newSearch}`, { replace: true });
    }
  }, [navigate, returnUrl, searchParams]);

  useEffect(() => {
    handleInitialStatusMessages();
  }, [handleInitialStatusMessages]);

  const onSubmit = async (data: LoginFormValues) => {
    form.clearErrors();
    setStatusAlert(null);
    setShowResendLinkForEmail(null);

    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      const result = await response.json();

      if (response.ok) {
        toast({
          title: "Login Successful",
          description: "Welcome back! Redirecting you...",
          variant: "default",
        });

        setTimeout(() => {
          if (returnUrl) {
            try {
              const returnUrlUrl = new URL(returnUrl, window.location.origin);
              if (returnUrlUrl.origin === window.location.origin) {
                navigate(returnUrlUrl.pathname + returnUrlUrl.search + returnUrlUrl.hash);
              } else {
                console.warn(`External redirect prevented: ${returnUrl}`);
                navigate('/');
              }
            } catch {
              console.error("Invalid returnUrl URL, redirecting to /");
              navigate('/');
            }
          } else {
            navigate('/');
          }
        }, 500);

      } else {
        const errorMessage = result.error || "Login failed.";
        form.setError("root", { message: errorMessage });
        if (response.status === 403 && errorMessage.toLowerCase().includes('email not confirmed')) {
          setShowResendLinkForEmail(data.email);
        }
      }

    } catch (error) {
      console.error("Login error:", error);
      form.setError("root", { message: "An unexpected error occurred. Please try again." });
    }
  };

  const handleResendConfirmation = async () => {
    if (!showResendLinkForEmail) return;

    setIsResendingConfirmation(true);

    if (!captchaToken) {
      toast({
        title: "CAPTCHA Required",
        description: "Please complete the CAPTCHA to resend confirmation.",
        variant: "destructive",
      });
      setIsResendingConfirmation(false);
      return;
    }

    try {
      const response = await fetch('/api/auth/resend-confirmation', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: showResendLinkForEmail,
          captchaToken,
        }),
      });

      const result = await response.json();

      if (response.ok) {
        toast({
          title: "Confirmation Email Sent",
          description: result.message || "If your email is registered, a new link has been sent.",
          variant: "default",
        });
        setShowResendLinkForEmail(null);
      } else {
        toast({
          title: "Error",
          description: result.message || "Failed to send confirmation email.",
          variant: "destructive",
        });
      }

    } catch (error) {
      console.error("Resend error:", error);
      toast({
        title: "Error",
        description: "Could not request confirmation email. Try again later.",
        variant: "destructive",
      });
    } finally {
      setIsResendingConfirmation(false);
    }
  };

  const handleCaptchaChange = (token: string | null) => {
    setCaptchaToken(token || '');
    setIsCaptchaVerified(!!token);
  };

  const handleCaptchaExpired = () => {
    setIsCaptchaVerified(false);
  };

  const renderStatusAlertComponent = () => {
    if (!statusAlert) return null;
    const Icon = statusAlert.type === 'success' ? CheckCircle : XCircle;
    const variant = statusAlert.type === 'success' ? 'default' : 'destructive';
    const alertClasses =
      statusAlert.type === 'success'
        ? "mb-4 bg-accent/10 border-accent/50 text-accent-foreground [&>svg]:text-accent"
        : "mb-4";

    return (
      <Alert variant={variant} className={alertClasses}>
        <Icon className="h-4 w-4" />
        <AlertTitle>{statusAlert.title}</AlertTitle>
        <AlertDescription>{statusAlert.description}</AlertDescription>
      </Alert>
    );
  };

  const registerHref = returnUrl ? `/register?returnUrl=${encodeURIComponent(returnUrl)}` : '/register';

  return {
    form,
    onSubmit,
    isSubmitting: form.formState.isSubmitting,
    showPassword,
    setShowPassword,
    renderStatusAlertComponent,
    registerHref,
    returnUrl,
    showResendLinkForEmail,
    handleResendConfirmation,
    isResendingConfirmation,
    handleCaptchaChange,
    handleCaptchaExpired,
    recaptchaRef,
    isCaptchaVerified,
    captchaToken,
  };
}
