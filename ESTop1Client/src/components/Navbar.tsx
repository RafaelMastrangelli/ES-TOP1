import { Link } from 'react-router-dom';
import { Target, Users, Trophy, Crown } from 'lucide-react';
import { Button } from './ui/button';
import { useAuth } from '../hooks/useAuth';

const Navbar = () => {
  const { isAuthenticated, user, logout } = useAuth();

  return (
    <nav className="sticky top-0 z-50 w-full border-b border-border bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container mx-auto flex h-16 items-center justify-between px-4">
        <Link to="/" className="flex items-center gap-2 font-bold text-xl">
          <Target className="h-6 w-6 text-primary" />
          <span className="bg-gradient-primary bg-clip-text text-transparent">ES-TOP1</span>
        </Link>

        {isAuthenticated && (
          <div className="hidden md:flex items-center gap-6">
            <Link to="/jogadores" className="flex items-center gap-2 text-sm font-medium transition-colors hover:text-primary">
              <Users className="h-4 w-4" />
              Jogadores
            </Link>
            <Link to="/times" className="flex items-center gap-2 text-sm font-medium transition-colors hover:text-primary">
              <Trophy className="h-4 w-4" />
              Times
            </Link>
            <Link to="/assinaturas" className="flex items-center gap-2 text-sm font-medium transition-colors hover:text-primary">
              <Crown className="h-4 w-4" />
              Assinaturas
            </Link>
          </div>
        )}

        <div className="flex items-center gap-2">
          {isAuthenticated ? (
            <>
              <span className="text-sm text-gray-600 hidden md:block">
                Olá, {user?.nome}
              </span>
              <Button variant="ghost" size="sm" onClick={logout}>
                Sair
              </Button>
            </>
          ) : (
            <>
              <Button variant="ghost" size="sm" asChild>
                <Link to="/inscricao">Inscrever-se</Link>
              </Button>
              <Button variant="ghost" size="sm" asChild>
                <Link to="/login">Entrar</Link>
              </Button>
              <Button size="sm" asChild>
                <Link to="/cadastro">Cadastrar</Link>
              </Button>
            </>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
