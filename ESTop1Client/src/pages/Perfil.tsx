import React from 'react';
import { useAuth } from '../hooks/useAuth';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Loader2, Lock, Crown, HelpCircle } from 'lucide-react';
import PerfilJogador from './PerfilJogador';
import PerfilOrganizacao from './PerfilOrganizacao';

const Perfil: React.FC = () => {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
          <p className="text-muted-foreground">Carregando perfil...</p>
        </div>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <Card className="w-full max-w-md animate-fade-in">
          <CardHeader className="text-center">
            <div className="flex justify-center mb-4">
              <div className="p-4 rounded-full bg-muted">
                <Lock className="h-12 w-12 text-muted-foreground" />
              </div>
            </div>
            <CardTitle className="text-2xl">Acesso Negado</CardTitle>
            <CardDescription>
              Você precisa estar logado para acessar seu perfil.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button 
              onClick={() => window.location.href = '/login'}
              className="w-full"
            >
              Fazer Login
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  // Renderizar o perfil baseado no tipo de usuário
  switch (user.tipo) {
    case 'Jogador':
      return <PerfilJogador />;
    case 'Organizacao':
      return <PerfilOrganizacao />;
    case 'Admin':
      return (
        <div className="min-h-screen bg-background flex items-center justify-center">
          <Card className="w-full max-w-md animate-fade-in">
            <CardHeader className="text-center">
              <div className="flex justify-center mb-4">
                <div className="p-4 rounded-full bg-primary/10">
                  <Crown className="h-12 w-12 text-primary" />
                </div>
              </div>
              <CardTitle className="text-2xl">Painel Administrativo</CardTitle>
              <CardDescription>
                Você tem acesso administrativo ao sistema.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              <Button 
                onClick={() => window.location.href = '/admin'}
                variant="destructive"
                className="w-full"
              >
                Painel Admin
              </Button>
              <Button 
                onClick={() => window.location.href = '/times'}
                className="w-full"
              >
                Gerenciar Times
              </Button>
            </CardContent>
          </Card>
        </div>
      );
    default:
      return (
        <div className="min-h-screen bg-background flex items-center justify-center">
          <Card className="w-full max-w-md animate-fade-in">
            <CardHeader className="text-center">
              <div className="flex justify-center mb-4">
                <div className="p-4 rounded-full bg-muted">
                  <HelpCircle className="h-12 w-12 text-muted-foreground" />
                </div>
              </div>
              <CardTitle className="text-2xl">Tipo de Usuário Inválido</CardTitle>
              <CardDescription>
                Seu tipo de usuário não é reconhecido pelo sistema.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Button 
                onClick={() => window.location.href = '/'}
                className="w-full"
              >
                Voltar ao Início
              </Button>
            </CardContent>
          </Card>
        </div>
      );
  }
};

export default Perfil;
