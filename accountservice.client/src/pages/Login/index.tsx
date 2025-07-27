import { Suspense, type FC } from 'react';
import { useLoginForm } from '@hooks/use-login-form';
import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Loader2, Eye, EyeOff, XCircle, MailQuestion } from 'lucide-react';
import ReCAPTCHA from 'react-google-recaptcha';

const GoogleSignInButton: FC<{ oidcOriginalRequest?: string | null }> = ({ oidcOriginalRequest }) => {
  if (import.meta.env.VITE_ENABLE_GOOGLE_SIGN_IN !== 'true') {
    return null;
  }

  let googleSignInHref = '/api/auth/google/signin';
  if (oidcOriginalRequest) {
    googleSignInHref += `?oidcOriginalRequest=${encodeURIComponent(oidcOriginalRequest)}`;
  }

  return (
    <Button variant="outline" className="w-full" asChild>
      <a href={googleSignInHref}>
        <svg className="mr-2 h-4 w-4" aria-hidden="true" focusable="false" data-prefix="fab" data-icon="google" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 488 512">
          <path fill="currentColor" d="M488 261.8C488 403.3...z"></path>
        </svg>
        Sign in with Google
      </a>
    </Button>
  );
};

const LoginFormComponent: FC = () => {
  const {
    form,
    onSubmit,
    isSubmitting,
    showPassword,
    setShowPassword,
    renderStatusAlertComponent,
    //registerHref,
    returnUrl,
    showResendLinkForEmail,
    handleResendConfirmation,
    isResendingConfirmation,
    handleCaptchaChange,
    handleCaptchaExpired,
    recaptchaRef,
    captchaToken
  } = useLoginForm();

  return (
    <Card>
      <CardHeader>
        <CardTitle>Sign in to Your Account</CardTitle>
        <CardDescription>Enter your email and password to access your dashboard.</CardDescription>
      </CardHeader>
      <CardContent>
        {renderStatusAlertComponent()}
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            {form.formState.errors.root && (
              <Alert variant="destructive" className="mb-4">
                <XCircle className="h-4 w-4" />
                <AlertTitle>Login Failed</AlertTitle>
                <AlertDescription>{form.formState.errors.root.message}</AlertDescription>
              </Alert>
            )}

            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Email</FormLabel>
                  <FormControl>
                    <Input placeholder="you@example.com" type="email" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="password"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Password</FormLabel>
                  <FormControl>
                    <div className="relative">
                      <Input
                        type={showPassword ? 'text' : 'password'}
                        placeholder="••••••••"
                        className="pr-10"
                        {...field}
                      />
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="absolute right-1 top-1/2 -translate-y-1/2 h-7 w-7 text-muted-foreground hover:text-foreground"
                        onClick={() => setShowPassword(!showPassword)}
                        tabIndex={-1}
                      >
                        {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                        <span className="sr-only">
                          {showPassword ? 'Hide password' : 'Show password'}
                        </span>
                      </Button>
                    </div>
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="flex items-center justify-between text-sm">
              <Link to="/forgot-password" className="text-primary hover:underline">
                Forgot password?
              </Link>
            </div>

            <Button type="submit" className="w-full" disabled={isSubmitting}>
              {isSubmitting ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Logging in...
                </>
              ) : (
                'Login'
              )}
            </Button>

            {import.meta.env.VITE_ENABLE_GOOGLE_SIGN_IN === 'true' && (
              <>
                <div className="relative my-4">
                  <div className="absolute inset-0 flex items-center">
                    <span className="w-full border-t" />
                  </div>
                  <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-background px-2 text-muted-foreground">
                      Or continue with
                    </span>
                  </div>
                </div>
                <GoogleSignInButton oidcOriginalRequest={returnUrl} />
              </>
            )}
          </form>
        </Form>

        {showResendLinkForEmail && (
          <div className="mt-6 p-4 border border-dashed border-primary/50 rounded-md bg-primary/5 ">
            <MailQuestion className="mx-auto h-8 w-8 text-primary mb-2" />
            <p className="text-sm text-primary-foreground/90 mb-1">
              It looks like the email <strong className="font-medium">{showResendLinkForEmail}</strong> is not confirmed yet.
            </p>
            <p className="text-xs text-primary-foreground/70 mb-3">
              Please check your inbox (and spam folder) for the confirmation link.
            </p>
            {import.meta.env.VITE_RECAPTCHA_SITE_KEY && (
              <ReCAPTCHA
                sitekey={import.meta.env.VITE_RECAPTCHA_SITE_KEY}
                ref={recaptchaRef}
                onChange={handleCaptchaChange}
                onExpired={handleCaptchaExpired}
              />
            )}
            <Button
              variant="link"
              className="text-sm text-primary hover:underline mt-2"
              onClick={handleResendConfirmation}
              disabled={isResendingConfirmation || isSubmitting || !captchaToken}
            >
              {isResendingConfirmation ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Sending...
                </>
              ) : (
                'Resend Confirmation Email'
              )}
            </Button>
          </div>
        )}
      </CardContent>

      <CardFooter className="flex flex-col items-center space-y-2">
       <p className="text-sm text-muted-foreground">
         Don't have an account?{' '}
         <Link to={'/register'} className="text-primary hover:underline">
           Register here
         </Link>
       </p>
      </CardFooter>
    </Card>
  );
};

const Login: FC = () => {
  const fallbackContent = (
    <Card>
      <CardHeader>
        <CardTitle>Loading...</CardTitle>
        <CardDescription>Please wait</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex justify-center items-center p-10">
          <Loader2 className="h-12 w-12 animate-spin text-primary" />
        </div>
      </CardContent>
    </Card>
  );

  return (
    <Suspense fallback={fallbackContent}>
        <div className="flex min-h-screen items-center justify-center p-4">
          <LoginFormComponent />
        </div>
    </Suspense>
  );
};

export default Login;
