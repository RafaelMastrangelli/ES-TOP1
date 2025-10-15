import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import { Loader2 } from 'lucide-react';

interface AuthGuardProps {
  children: React.ReactNode;
  requireAuth?: boolean;
  allowedUserTypes?: string[];
}

export const AuthGuard = ({ 
  children, 
  requireAuth = true, 
  allowedUserTypes = [] 
}: AuthGuardProps) => {
  const { isAuthenticated, user, isLoading } = useAuth();
  const location = useLocation();

  // Mostrar loading enquanto verifica autenticação
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
          <p className="text-muted-foreground">Verificando autenticação...</p>
        </div>
      </div>
    );
  }

  // Se a rota requer autenticação mas o usuário não está autenticado
  if (requireAuth && !isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Se a rota é apenas para usuários não autenticados mas o usuário está logado
  if (!requireAuth && isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  // Se há restrições de tipo de usuário
  if (requireAuth && allowedUserTypes.length > 0 && user) {
    if (!allowedUserTypes.includes(user.tipo)) {
      return (
        <div className="min-h-screen flex items-center justify-center">
          <div className="text-center">
            <h1 className="text-2xl font-bold text-destructive mb-2">
              Acesso Negado
            </h1>
            <p className="text-muted-foreground mb-4">
              Você não tem permissão para acessar esta página.
            </p>
            <p className="text-sm text-muted-foreground">
              Tipo de usuário: {user.tipo}
            </p>
          </div>
        </div>
      );
    }
  }

  return <>{children}</>;
};

// Componente para rotas que requerem autenticação
export const ProtectedRoute = ({ 
  children, 
  allowedUserTypes = [] 
}: Omit<AuthGuardProps, 'requireAuth'>) => (
  <AuthGuard requireAuth={true} allowedUserTypes={allowedUserTypes}>
    {children}
  </AuthGuard>
);

// Componente para rotas que são apenas para usuários não autenticados
export const PublicOnlyRoute = ({ children }: { children: React.ReactNode }) => (
  <AuthGuard requireAuth={false}>
    {children}
  </AuthGuard>
);

// Componente para rotas específicas de organizações
export const OrganizationRoute = ({ children }: { children: React.ReactNode }) => (
  <AuthGuard requireAuth={true} allowedUserTypes={['Organizacao', 'Admin']}>
    {children}
  </AuthGuard>
);

// Componente para rotas específicas de jogadores
export const PlayerRoute = ({ children }: { children: React.ReactNode }) => (
  <AuthGuard requireAuth={true} allowedUserTypes={['Jogador', 'Admin']}>
    {children}
  </AuthGuard>
);
