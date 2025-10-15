import { Link } from 'react-router-dom';
import { Target, Users, Trophy, Crown, LogOut, User } from 'lucide-react';
import { Button } from './ui/button';
import { useAuth } from '../hooks/useAuth';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from './ui/dropdown-menu';

interface NavbarProps {
  showNavigation?: boolean;
  embedded?: boolean; // Para quando a navbar está integrada em outro container
}

const Navbar = ({ showNavigation = true, embedded = false }: NavbarProps) => {
  const { isAuthenticated, user, logout } = useAuth();

  const handleLogout = () => {
    logout();
  };

  // Função para verificar se o usuário tem acesso a uma funcionalidade
  const hasAccess = (userType: string) => {
    if (!user) return false;
    
    // Jogadores têm acesso a todas as funcionalidades básicas
    if (userType === 'Jogador') return true;
    
    // Organizações têm acesso a todas as funcionalidades
    if (userType === 'Organização') return true;
    
    // Admins têm acesso total
    if (userType === 'Admin') return true;
    
    return false;
  };

  const navbarContent = (
    <>
      <Link to="/" className="flex items-center gap-2 font-bold text-xl">
        <Target className="h-6 w-6 text-primary" />
        <span className="bg-gradient-primary bg-clip-text text-transparent">ES-TOP1</span>
      </Link>

      {isAuthenticated && showNavigation && (
        <div className="hidden md:flex items-center gap-6">
          {/* Jogadores - apenas para Organizações e Admins */}
          {(user?.tipo === 'Organização' || user?.tipo === 'Admin') && (
            <Link to="/jogadores" className="flex items-center gap-2 text-sm font-medium transition-colors hover:text-primary">
              <Users className="h-4 w-4" />
              Jogadores
            </Link>
          )}
          
          {/* Times - disponível para todos os usuários logados */}
          <Link to="/times" className="flex items-center gap-2 text-sm font-medium transition-colors hover:text-primary">
            <Trophy className="h-4 w-4" />
            Times
          </Link>
          
          {/* Assinaturas - disponível para todos os usuários logados */}
          <Link to="/assinaturas" className="flex items-center gap-2 text-sm font-medium transition-colors hover:text-primary">
            <Crown className="h-4 w-4" />
            Assinaturas
          </Link>
        </div>
      )}

      <div className="flex items-center gap-2">
        {isAuthenticated ? (
          <div className="flex items-center space-x-4">
            {/* User Info */}
            <div className="hidden sm:flex items-center space-x-2 text-sm">
              <span className="text-muted-foreground">Olá,</span>
              <span className="font-medium">{user?.nome}</span>
              <span className="px-2 py-1 bg-primary/10 text-primary text-xs rounded-full">
                {user?.tipo}
              </span>
            </div>

            {/* User Menu */}
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="sm" className="relative h-8 w-8 rounded-full">
                  <User className="h-4 w-4" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent className="w-56" align="end" forceMount>
                <DropdownMenuLabel className="font-normal">
                  <div className="flex flex-col space-y-1">
                    <p className="text-sm font-medium leading-none">{user?.nome}</p>
                    <p className="text-xs leading-none text-muted-foreground">
                      {user?.email}
                    </p>
                    <p className="text-xs leading-none text-muted-foreground">
                      Tipo: {user?.tipo}
                    </p>
                  </div>
                </DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem asChild>
                  <Link to="/perfil" className="flex items-center">
                    <User className="mr-2 h-4 w-4" />
                    <span>Meu Perfil</span>
                  </Link>
                </DropdownMenuItem>
                <DropdownMenuItem onClick={handleLogout}>
                  <LogOut className="mr-2 h-4 w-4" />
                  <span>Sair</span>
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        ) : (
          <div className="flex items-center space-x-2">
            <Button variant="ghost" size="sm" asChild>
              <Link to="/inscricao">Inscrever-se</Link>
            </Button>
            <Button variant="ghost" size="sm" asChild>
              <Link to="/login">Entrar</Link>
            </Button>
            <Button size="sm" asChild>
              <Link to="/cadastro">Cadastrar</Link>
            </Button>
          </div>
        )}
      </div>
    </>
  );

  if (embedded) {
    return (
      <div className="flex items-center justify-between w-full">
        {navbarContent}
      </div>
    );
  }

  return (
    <nav className="sticky top-0 z-50 w-full border-b border-border bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container mx-auto flex h-16 items-center justify-between px-4">
        {navbarContent}
      </div>
    </nav>
  );
};

export default Navbar;
