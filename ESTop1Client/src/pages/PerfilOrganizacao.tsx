import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Time, Jogador } from '../types';

const PerfilOrganizacao: React.FC = () => {
  const { user, assinatura } = useAuth();
  const [time, setTime] = useState<Time | null>(null);
  const [jogadores, setJogadores] = useState<Jogador[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const carregarPerfil = async () => {
      try {
        setLoading(true);
        setError(null);

        // Buscar dados da organização/time associado ao usuário
        const time = await api.times.meuTime();
        if (time) {
          setTime(time);
          setJogadores(time.jogadores || []);
        }
      } catch (err: any) {
        setError(err.response?.data?.message || 'Erro ao carregar perfil');
      } finally {
        setLoading(false);
      }
    };

    if (user?.tipo === 'Organizacao') {
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

  if (!time) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center max-w-md mx-auto p-6">
          <div className="text-gray-400 text-6xl mb-4">🏢</div>
          <h2 className="text-2xl font-bold text-gray-900 mb-4">Time não encontrado</h2>
          <p className="text-gray-600 mb-6">
            Parece que você ainda não tem um time criado. 
            Crie seu time para começar a gerenciar jogadores.
          </p>
          <button 
            onClick={() => window.location.href = '/times'}
            className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 font-medium"
          >
            Criar Time
          </button>
        </div>
      </div>
    );
  }

  const jogadoresContratados = jogadores.filter(j => j.disponibilidade === 'EmTime');
  const jogadoresLivres = jogadores.filter(j => j.disponibilidade === 'Livre');

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header do Perfil */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 mb-6">
          <div className="px-6 py-8">
            <div className="flex flex-col sm:flex-row items-start sm:items-center space-y-4 sm:space-y-0 sm:space-x-6">
              {/* Logo do Time */}
              <div className="flex-shrink-0">
                <div className="w-24 h-24 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center text-white text-2xl font-bold shadow-lg">
                  {time.nome.charAt(0)}
                </div>
              </div>
              
              {/* Informações Básicas */}
              <div className="flex-1 min-w-0">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                  <div>
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">
                      {time.nome}
                    </h1>
                    <div className="flex flex-wrap items-center gap-4 text-sm text-gray-600">
                      <span className="flex items-center">
                        <span className="w-2 h-2 bg-blue-500 rounded-full mr-2"></span>
                        {time.tier ? `Tier ${time.tier}` : 'Sem tier'}
                      </span>
                      <span className="flex items-center">
                        <span className={`w-2 h-2 rounded-full mr-2 ${
                          time.contratando ? 'bg-green-500' : 'bg-red-500'
                        }`}></span>
                        {time.contratando ? 'Contratando' : 'Não contratando'}
                      </span>
                      <span>🇧🇷 {time.pais}</span>
                      <span>{jogadores.length} jogadores</span>
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

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Conteúdo Principal */}
          <div className="lg:col-span-3 space-y-6">
            {/* Estatísticas do Time */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-4">Estatísticas do Time</h2>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div className="text-center">
                  <div className="text-2xl font-bold text-blue-600">{jogadores.length}</div>
                  <div className="text-sm text-gray-600">Total de Jogadores</div>
                </div>
                <div className="text-center">
                  <div className="text-2xl font-bold text-green-600">{jogadoresContratados.length}</div>
                  <div className="text-sm text-gray-600">Contratados</div>
                </div>
                <div className="text-center">
                  <div className="text-2xl font-bold text-yellow-600">{jogadoresLivres.length}</div>
                  <div className="text-sm text-gray-600">Em Teste</div>
                </div>
                <div className="text-center">
                  <div className="text-2xl font-bold text-purple-600">
                    {jogadores.reduce((acc, jogador) => acc + jogador.valorDeMercado, 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                  </div>
                  <div className="text-sm text-gray-600">Valor Total</div>
                </div>
              </div>
            </div>

            {/* Lista de Jogadores */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-xl font-semibold text-gray-900">Jogadores</h2>
                <button className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 text-sm">
                  Adicionar Jogador
                </button>
              </div>
              
              {jogadores.length > 0 ? (
                <div className="space-y-3">
                  {jogadores.map((jogador) => (
                    <div key={jogador.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
                      <div className="flex items-center space-x-4">
                        <img
                          src={jogador.fotoUrl || '/player-default.jpg'}
                          alt={jogador.apelido}
                          className="w-12 h-12 rounded-full object-cover"
                        />
                        <div>
                          <h3 className="font-semibold text-gray-900">{jogador.apelido}</h3>
                          <p className="text-sm text-gray-600">
                            {jogador.funcaoPrincipal} • {jogador.status}
                          </p>
                        </div>
                      </div>
                      <div className="flex items-center space-x-4">
                        <div className="text-right">
                          <div className="font-semibold text-gray-900">
                            {jogador.valorDeMercado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                          </div>
                          <div className={`text-sm ${
                            jogador.disponibilidade === 'EmTime' ? 'text-green-600' : 'text-yellow-600'
                          }`}>
                            {jogador.disponibilidade}
                          </div>
                        </div>
                        <button className="text-gray-400 hover:text-gray-600">
                          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z" />
                          </svg>
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="text-center py-8">
                  <div className="text-gray-400 text-4xl mb-2">👥</div>
                  <p className="text-gray-600 mb-4">Nenhum jogador cadastrado</p>
                  <button className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700">
                    Adicionar Primeiro Jogador
                  </button>
                </div>
              )}
            </div>

            {/* Informações do Time */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h2 className="text-xl font-semibold text-gray-900 mb-4">Informações do Time</h2>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Nome</label>
                  <p className="text-gray-900">{time.nome}</p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">País</label>
                  <p className="text-gray-900">🇧🇷 {time.pais}</p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Tier</label>
                  <p className="text-gray-900">{time.tier ? `Tier ${time.tier}` : 'Não definido'}</p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Status de Contratação</label>
                  <p className="text-gray-900">{time.contratando ? 'Contratando' : 'Não contratando'}</p>
                </div>
              </div>
            </div>
          </div>

          {/* Sidebar */}
          <div className="space-y-6">
            {/* Ações Rápidas */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Ações</h3>
              <div className="space-y-3">
                <button className="w-full bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors">
                  Editar Time
                </button>
                <button className="w-full bg-green-600 text-white py-2 px-4 rounded-lg hover:bg-green-700 transition-colors">
                  Adicionar Jogador
                </button>
                <button className="w-full bg-gray-100 text-gray-700 py-2 px-4 rounded-lg hover:bg-gray-200 transition-colors">
                  Buscar Jogadores
                </button>
                <button className="w-full bg-gray-100 text-gray-700 py-2 px-4 rounded-lg hover:bg-gray-200 transition-colors">
                  Estatísticas
                </button>
              </div>
            </div>

            {/* Status da Conta */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Status da Conta</h3>
              <div className="space-y-3">
                <div className="flex justify-between">
                  <span className="text-gray-600">Tipo de Usuário</span>
                  <span className="font-medium text-gray-900">Organização</span>
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

            {/* Limites do Plano */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Limites do Plano</h3>
              <div className="space-y-3">
                <div className="flex justify-between">
                  <span className="text-gray-600">Jogadores</span>
                  <span className="font-medium text-gray-900">
                    {jogadores.length} / {assinatura?.plano === 'Enterprise' ? '∞' : '10'}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Busca IA</span>
                  <span className={`font-medium ${
                    assinatura?.plano !== 'Gratuito' ? 'text-green-600' : 'text-red-600'
                  }`}>
                    {assinatura?.plano !== 'Gratuito' ? 'Disponível' : 'Bloqueado'}
                  </span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">API</span>
                  <span className={`font-medium ${
                    assinatura?.plano === 'Enterprise' ? 'text-green-600' : 'text-red-600'
                  }`}>
                    {assinatura?.plano === 'Enterprise' ? 'Disponível' : 'Bloqueado'}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PerfilOrganizacao;
