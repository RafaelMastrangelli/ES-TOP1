import { Link } from 'react-router-dom';
import { useAuth } from '@/hooks/useAuth';
import Navbar from './Navbar';

interface PublicLayoutProps {
  children: React.ReactNode;
}

export const PublicLayout = ({ children }: PublicLayoutProps) => {
  const { isAuthenticated, logout } = useAuth();

  const handleLogout = () => {
    logout();
  };

  return (
    <div className="min-h-screen bg-background">
      {/* Navbar */}
      <Navbar showNavigation={isAuthenticated} />

      {/* Main Content */}
      <main className="flex-1">
        {children}
      </main>

      {/* Footer */}
      <footer className="border-t bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div className="container mx-auto px-4 py-8">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div className="space-y-4">
              <div className="flex items-center space-x-2">
                <div className="h-6 w-6 bg-primary rounded-md flex items-center justify-center">
                  <span className="text-primary-foreground font-bold text-xs">ES</span>
                </div>
                <span className="font-bold">sada</span>
              </div>
              <p className="text-sm text-muted-foreground">
                A plataforma definitiva para conectar jogadores e times de CS2.
              </p>
            </div>
            
            <div className="space-y-4">
              <h3 className="text-sm font-semibold">Jogadores</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li><Link to="/jogadores" className="hover:text-foreground transition-colors">Buscar Jogadores</Link></li>
                <li><Link to="/inscricao" className="hover:text-foreground transition-colors">Inscrever-se</Link></li>
              </ul>
            </div>
            
            <div className="space-y-4">
              <h3 className="text-sm font-semibold">Times</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li><Link to="/times" className="hover:text-foreground transition-colors">Buscar Times</Link></li>
                <li><Link to="/assinaturas" className="hover:text-foreground transition-colors">Planos</Link></li>
              </ul>
            </div>
            
            <div className="space-y-4">
              <h3 className="text-sm font-semibold">Conta</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                {isAuthenticated ? (
                  <>
                    <li><button onClick={handleLogout} className="hover:text-foreground transition-colors">Sair</button></li>
                  </>
                ) : (
                  <>
                    <li><Link to="/login" className="hover:text-foreground transition-colors">Entrar</Link></li>
                    <li><Link to="/cadastro" className="hover:text-foreground transition-colors">Cadastrar</Link></li>
                  </>
                )}
              </ul>
            </div>
          </div>
          
          <div className="mt-8 pt-8 border-t text-center text-sm text-muted-foreground">
            <p>&copy; 2024 ESTop1. Todos os direitos reservados.</p>
          </div>
        </div>
      </footer>
    </div>
  );
};
