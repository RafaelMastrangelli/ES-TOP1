import React from 'react';
import { useAuth } from '../hooks/useAuth';
import PerfilJogador from './PerfilJogador';
import PerfilOrganizacao from './PerfilOrganizacao';

const Perfil: React.FC = () => {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Carregando...</p>
        </div>
      </div>
    );
  }

  if (!user) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center max-w-md mx-auto p-6">
          <div className="text-gray-400 text-6xl mb-4">🔒</div>
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Acesso Negado</h2>
          <p className="text-gray-600 mb-6">
            Você precisa estar logado para acessar seu perfil.
          </p>
          <button 
            onClick={() => window.location.href = '/login'}
            className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium"
          >
            Fazer Login
          </button>
        </div>
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
        <div className="min-h-screen bg-gray-50 flex items-center justify-center">
          <div className="text-center max-w-md mx-auto p-6">
            <div className="text-gray-400 text-6xl mb-4">👑</div>
            <h2 className="text-2xl font-bold text-gray-900 mb-4">Painel Administrativo</h2>
            <p className="text-gray-600 mb-6">
              Você tem acesso administrativo ao sistema.
            </p>
            <div className="space-y-3">
              <button 
                onClick={() => window.location.href = '/admin'}
                className="w-full bg-red-600 text-white px-6 py-3 rounded-lg hover:bg-red-700 font-medium"
              >
                Painel Admin
              </button>
              <button 
                onClick={() => window.location.href = '/times'}
                className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium"
              >
                Gerenciar Times
              </button>
            </div>
          </div>
        </div>
      );
    default:
      return (
        <div className="min-h-screen bg-gray-50 flex items-center justify-center">
          <div className="text-center max-w-md mx-auto p-6">
            <div className="text-gray-400 text-6xl mb-4">❓</div>
            <h2 className="text-2xl font-bold text-gray-900 mb-4">Tipo de Usuário Inválido</h2>
            <p className="text-gray-600 mb-6">
              Seu tipo de usuário não é reconhecido pelo sistema.
            </p>
            <button 
              onClick={() => window.location.href = '/'}
              className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium"
            >
              Voltar ao Início
            </button>
          </div>
        </div>
      );
  }
};

export default Perfil;
