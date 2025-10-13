import { ReactNode } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Target } from 'lucide-react';

interface AuthFormProps {
  title: string;
  description: string;
  children: ReactNode;
}

const AuthForm = ({ title, description, children }: AuthFormProps) => {
  return (
    <div className="min-h-screen bg-background">
      <div className="container mx-auto px-4 py-16">
        <div className="max-w-md mx-auto">
          <Card className="border-border bg-card animate-fade-in">
            <CardHeader className="text-center space-y-4">
              <div className="flex justify-center">
                <div className="p-3 rounded-full bg-primary/10">
                  <Target className="h-8 w-8 text-primary" />
                </div>
              </div>
              <div>
                <CardTitle className="text-2xl font-bold">{title}</CardTitle>
                <CardDescription>{description}</CardDescription>
              </div>
            </CardHeader>
            
            <CardContent>
              {children}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default AuthForm;
