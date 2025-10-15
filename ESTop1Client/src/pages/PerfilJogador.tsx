import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Jogador, Estatistica } from '../types';

const PerfilJogador: React.FC = () => {
  const { user, assinatura } = useAuth();
  const [jogador, setJogador] = useState<Jogador | null>(null);
  const [estatisticas, setEstatisticas] = useState<Estatistica[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const carregarPerfil = async () => {
      try {
        setLoading(true);
        setError(null);

        // Buscar dados do jogador associado ao usuário
        const jogador = await api.jogadores.meuPerfil();
        if (jogador) {
          setJogador(jogador);
          setEstatisticas(jogador.estatisticas || []);
        }
      } catch (err: any) {
        setError(err.response?.data?.message || 'Erro ao carregar perfil');
      } finally {
        setLoading(false);
      }
    };

    if (user?.tipo === 'Jogador') {
      carregarPerfil();
    }
  }, [user]);

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Carregando perfil...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="text-red-500 text-xl mb-4">⚠️</div>
          <h2 className="text-xl font-semibold text-gray-900 mb-2">Erro ao carregar perfil</h2>
          <p className="text-gray-600 mb-4">{error}</p>
          <button 
            onClick={() => window.location.reload()}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
          >
            Tentar novamente
          </button>
        </div>
      </div>
    );
  }

  if (!jogador) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center max-w-md mx-auto p-6">
          <div className="text-gray-400 text-6xl mb-4">👤</div>
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Perfil não encontrado</h2>
          <p className="text-gray-600 mb-6">
            Parece que você ainda não tem um perfil de jogador criado. 
            Complete seu cadastro para começar a usar a plataforma.
          </p>
          <button 
            onClick={() => window.location.href = '/inscricao'}
            className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium"
          >
            Criar Perfil
          </button>
        </div>
      </div>
    );
  }

  const estatisticaGeral = estatisticas.find(stat => stat.periodo === 'Geral') || estatisticas[0];

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header do Perfil */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 mb-6">
          <div className="px-6 py-8">
            <div className="flex flex-col sm:flex-row items-start sm:items-center space-y-4 sm:space-y-0 sm:space-x-6">
              {/* Foto do Jogador */}
              <div className="flex-shrink-0">
                <img
                  src={jogador.fotoUrl || '/player-default.jpg'}
                  alt={jogador.apelido}
                  className="w-24 h-24 rounded-full object-cover border-4 border-white shadow-lg"
                />
              </div>
              
              {/* Informações Básicas */}
              <div className="flex-1 min-w-0">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                  <div>
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">
                      {jogador.apelido}
                    </h1>
                    <div className="flex flex-wrap items-center gap-4 text-sm text-gray-600">
                      <span className="flex items-center">
                        <span className="w-2 h-2 bg-green-500 rounded-full mr-2"></span>
                        {jogador.status}
                      </span>
                      <span className="flex items-center">
                        <span className="w-2 h-2 bg-blue-500 rounded-full mr-2"></span>
                        {jogador.disponibilidade}
                      </span>
                      <span>🇧🇷 {jogador.pais}</span>
                      <span>{jogador.idade} anos</span>
                    </div>
                  </div>
                  
                  {/* Status da Assinatura */}
                  <div className="mt-4 sm:mt-0">
                    <div className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${
                      assinatura?.status === 'Ativa' 
                        ? 'bg-green-100 text-green-800' 
                        : 'bg-yellow-100 text-yellow-800'
                    }`}>
                      {assinatura?.status === 'Ativa' ? '✅' : '⚠️'} 
                      {assinatura?.plano || 'Gratuito'}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Informações Detalhadas */}
          <div className="lg:col-span-2 space-y-6">
            {/* Estatísticas */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-4">Estatísticas</h2>
              {estatisticaGeral ? (
                <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                  <div className="text-center">
                    <div className="text-2xl font-bold text-blue-600">{estatisticaGeral.rating}</div>
                    <div className="text-sm text-gray-600">Rating</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-green-600">{estatisticaGeral.kd}</div>
                    <div className="text-sm text-gray-600">K/D</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-purple-600">{estatisticaGeral.partidasJogadas}</div>
                    <div className="text-sm text-gray-600">Partidas</div>
                  </div>
                  <div className="text-center">
                    <div className="text-2xl font-bold text-orange-600">
                      {jogador.valorDeMercado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                    </div>
                    <div className="text-sm text-gray-600">Valor</div>
                  </div>
                </div>
              ) : (
                <div className="text-center py-8">
                  <div className="text-gray-400 text-4xl mb-2">📊</div>
                  <p className="text-gray-600">Nenhuma estatística disponível</p>
                </div>
              )}
            </div>

            {/* Informações do Jogador */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-4">Informações</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Função Principal</label>
                  <p className="text-gray-900">{jogador.funcaoPrincipal}</p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Status</label>
                  <p className="text-gray-900">{jogador.status}</p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Disponibilidade</label>
                  <p className="text-gray-900">{jogador.disponibilidade}</p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">País</label>
                  <p className="text-gray-900">🇧🇷 {jogador.pais}</p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Idade</label>
                  <p className="text-gray-900">{jogador.idade} anos</p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Valor de Mercado</label>
                  <p className="text-gray-900">
                    {jogador.valorDeMercado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                  </p>
                </div>
              </div>
            </div>

            {/* Time Atual */}
            {jogador.timeAtual && (
              <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
                <h2 className="text-xl font-semibold text-gray-900 mb-4">Time Atual</h2>
                <div className="flex items-center space-x-4">
                  <div className="w-12 h-12 bg-gray-200 rounded-lg flex items-center justify-center">
                    <span className="text-gray-600 font-semibold">
                      {jogador.timeAtual.nome.charAt(0)}
                    </span>
                  </div>
                  <div>
                    <h3 className="font-semibold text-gray-900">{jogador.timeAtual.nome}</h3>
                    <p className="text-sm text-gray-600">
                      {jogador.timeAtual.tier && `Tier ${jogador.timeAtual.tier}`} • 
                      {jogador.timeAtual.contratando ? ' Contratando' : ' Não contratando'}
                    </p>
                  </div>
                </div>
              </div>
            )}
          </div>

          {/* Sidebar */}
          <div className="space-y-6">
            {/* Ações Rápidas */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Ações</h3>
              <div className="space-y-3">
                <button className="w-full bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors">
                  Editar Perfil
                </button>
                <button className="w-full bg-gray-100 text-gray-700 py-2 px-4 rounded-lg hover:bg-gray-200 transition-colors">
                  Ver Estatísticas
                </button>
                <button className="w-full bg-gray-100 text-gray-700 py-2 px-4 rounded-lg hover:bg-gray-200 transition-colors">
                  Buscar Times
                </button>
              </div>
            </div>

            {/* Status da Conta */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Status da Conta</h3>
              <div className="space-y-3">
                <div className="flex justify-between">
                  <span className="text-gray-600">Tipo de Usuário</span>
                  <span className="font-medium text-gray-900">Jogador</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Plano</span>
                  <span className="font-medium text-gray-900">{assinatura?.plano || 'Gratuito'}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Status</span>
                  <span className={`font-medium ${
                    assinatura?.status === 'Ativa' ? 'text-green-600' : 'text-yellow-600'
                  }`}>
                    {assinatura?.status || 'Inativa'}
                  </span>
                </div>
                {assinatura?.dataFim && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">Válido até</span>
                    <span className="font-medium text-gray-900">
                      {new Date(assinatura.dataFim).toLocaleDateString('pt-BR')}
                    </span>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PerfilJogador;
