// src/pages/ChangePinPage.tsx
import { Suspense } from "react";
import { Loader2, ArrowLeft } from "lucide-react";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import ChangePinForm from "@/components/profile/change-pin-form";
import { Link } from "react-router-dom";

const ChangePinPage: React.FC = () => {
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
        <div className="w-full max-w-md space-y-4">
          {/* Back Button */}
          <Link to="/profile">
            <Button variant="ghost" className="flex items-center gap-2 mb-4">
              <ArrowLeft className="h-4 w-4" />
              Back to Profile
            </Button>
          </Link>

          <ChangePinForm />
        </div>
      </div>
    </Suspense>
  );
};

export default ChangePinPage;
