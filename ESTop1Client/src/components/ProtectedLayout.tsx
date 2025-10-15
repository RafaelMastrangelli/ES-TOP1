import { useAuth } from '@/hooks/useAuth';
import Navbar from './Navbar';

interface ProtectedLayoutProps {
  children: React.ReactNode;
}

export const ProtectedLayout = ({ children }: ProtectedLayoutProps) => {
  return (
    <div className="min-h-screen bg-background">
      {/* Navbar */}
      <Navbar showNavigation={true} />

      {/* Main Content */}
      <main className="flex-1">
        {children}
      </main>
    </div>
  );
};
