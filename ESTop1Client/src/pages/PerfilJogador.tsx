import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Jogador, Estatistica } from '../types';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { Loader2, User, Edit, BarChart3, Search, CheckCircle, AlertCircle, Trophy, Target, TrendingUp, Calendar, MapPin, DollarSign, Users, Award, Clock, Star } from 'lucide-react';
import EditarPerfilJogador from '../components/EditarPerfilJogador';

const PerfilJogador: React.FC = () => {
  const { user, assinatura } = useAuth();
  const [jogador, setJogador] = useState<Jogador | null>(null);
  const [estatisticas, setEstatisticas] = useState<Estatistica[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editandoPerfil, setEditandoPerfil] = useState(false);
  const [criandoPerfil, setCriandoPerfil] = useState(false);

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

  const handleCriarPerfil = async () => {
    try {
      setCriandoPerfil(true);
      const novoJogador = await api.jogadores.criarPerfil();
      setJogador(novoJogador);
      setEstatisticas(novoJogador.estatisticas || []);
      setError(null);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao criar perfil');
    } finally {
      setCriandoPerfil(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="flex flex-col items-center gap-4">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
          <p className="text-muted-foreground">Carregando perfil...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <Card className="w-full max-w-md animate-fade-in">
          <CardHeader className="text-center">
            <div className="flex justify-center mb-4">
              <div className="p-4 rounded-full bg-destructive/10">
                <AlertCircle className="h-12 w-12 text-destructive" />
              </div>
            </div>
            <CardTitle className="text-xl">Erro ao carregar perfil</CardTitle>
            <CardDescription>{error}</CardDescription>
          </CardHeader>
          <CardContent>
            <Button 
              onClick={() => window.location.reload()}
              className="w-full"
            >
              Tentar novamente
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (!jogador) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <Card className="w-full max-w-md animate-fade-in">
          <CardHeader className="text-center">
            <div className="flex justify-center mb-4">
              <div className="p-4 rounded-full bg-muted">
                <User className="h-12 w-12 text-muted-foreground" />
              </div>
            </div>
            <CardTitle className="text-2xl">Perfil não encontrado</CardTitle>
            <CardDescription>
              Parece que você ainda não tem um perfil de jogador criado. 
              Clique no botão abaixo para criar seu perfil automaticamente.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            <Button 
              onClick={handleCriarPerfil}
              disabled={criandoPerfil}
              className="w-full"
            >
              {criandoPerfil ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Criando Perfil...
                </>
              ) : (
                'Criar Perfil Automaticamente'
              )}
            </Button>
            <Button 
              variant="outline"
              onClick={() => window.location.href = '/inscricao'}
              className="w-full"
            >
              Criar Perfil Manualmente
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  const estatisticaGeral = estatisticas.find(stat => stat.periodo === 'Geral') || estatisticas[0];

  const handlePerfilAtualizado = (jogadorAtualizado: Jogador) => {
    setJogador(jogadorAtualizado);
    setEstatisticas(jogadorAtualizado.estatisticas || []);
  };

  return (
    <div className="min-h-screen bg-background">
      <main className="container mx-auto px-4 py-8">
        <div className="max-w-6xl mx-auto">
          {/* Header do Perfil */}
          <Card className="mb-8 animate-fade-in">
            <CardContent className="p-8">
              <div className="flex flex-col sm:flex-row items-start sm:items-center space-y-4 sm:space-y-0 sm:space-x-6">
                {/* Foto do Jogador */}
                <div className="flex-shrink-0">
                  <img
                    src={jogador.fotoUrl || '/player-default.jpg'}
                    alt={jogador.apelido}
                    className="w-24 h-24 rounded-full object-cover border-4 border-card shadow-lg"
                  />
                </div>
                
                {/* Informações Básicas */}
                <div className="flex-1 min-w-0">
                  <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                    <div>
                      <h1 className="text-3xl font-bold mb-2">
                        {jogador.apelido}
                      </h1>
                      <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground">
                        <span className="flex items-center">
                          <span className="w-2 h-2 bg-success rounded-full mr-2"></span>
                          {jogador.status}
                        </span>
                        <span className="flex items-center">
                          <span className="w-2 h-2 bg-primary rounded-full mr-2"></span>
                          {jogador.disponibilidade}
                        </span>
                        <span>🇧🇷 {jogador.pais}</span>
                        <span>{jogador.idade} anos</span>
                      </div>
                    </div>
                    
                    {/* Status da Assinatura e Botão Editar */}
                    <div className="mt-4 sm:mt-0 flex flex-col sm:flex-row items-end gap-3">
                      <Badge variant={assinatura?.status === 'Ativa' ? 'default' : 'secondary'} className="text-sm">
                        {assinatura?.status === 'Ativa' ? (
                          <CheckCircle className="h-3 w-3 mr-1" />
                        ) : (
                          <AlertCircle className="h-3 w-3 mr-1" />
                        )}
                        {assinatura?.plano || 'Gratuito'}
                      </Badge>
                      <Button
                        onClick={() => setEditandoPerfil(true)}
                        size="sm"
                        className="w-full sm:w-auto"
                      >
                        <Edit className="h-4 w-4 mr-2" />
                        Editar Perfil
                      </Button>
                    </div>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* Informações Detalhadas */}
            <div className="lg:col-span-2 space-y-6">
              {/* Estatísticas */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <BarChart3 className="h-5 w-5" />
                    Estatísticas
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {estatisticaGeral ? (
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                      <div className="text-center">
                        <div className="text-2xl font-bold text-primary">{estatisticaGeral.rating}</div>
                        <div className="text-sm text-muted-foreground">Rating</div>
                      </div>
                      <div className="text-center">
                        <div className="text-2xl font-bold text-success">{estatisticaGeral.kd}</div>
                        <div className="text-sm text-muted-foreground">K/D</div>
                      </div>
                      <div className="text-center">
                        <div className="text-2xl font-bold text-secondary">{estatisticaGeral.partidasJogadas}</div>
                        <div className="text-sm text-muted-foreground">Partidas</div>
                      </div>
                      <div className="text-center">
                        <div className="text-2xl font-bold text-warning">
                          {jogador.valorDeMercado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                        </div>
                        <div className="text-sm text-muted-foreground">Valor</div>
                      </div>
                    </div>
                  ) : (
                    <div className="text-center py-8">
                      <BarChart3 className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
                      <p className="text-muted-foreground">Nenhuma estatística disponível</p>
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Informações do Jogador */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <User className="h-5 w-5" />
                    Informações
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">Função Principal</label>
                      <p className="text-foreground">{jogador.funcaoPrincipal}</p>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">Status</label>
                      <p className="text-foreground">{jogador.status}</p>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">Disponibilidade</label>
                      <p className="text-foreground">{jogador.disponibilidade}</p>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">País</label>
                      <p className="text-foreground">🇧🇷 {jogador.pais}</p>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">Idade</label>
                      <p className="text-foreground">{jogador.idade} anos</p>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">Valor de Mercado</label>
                      <p className="text-foreground">
                        {jogador.valorDeMercado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                      </p>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Time Atual */}
              {jogador.timeAtual && (
                <Card className="animate-fade-in">
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <Users className="h-5 w-5" />
                      Time Atual
                    </CardTitle>
                  </CardHeader>
                  <CardContent>
                    <div className="flex items-center space-x-4">
                      <div className="w-12 h-12 bg-gradient-to-br from-primary to-secondary rounded-lg flex items-center justify-center">
                        <span className="text-white font-semibold">
                          {jogador.timeAtual.nome.charAt(0)}
                        </span>
                      </div>
                      <div>
                        <h3 className="font-semibold text-foreground">{jogador.timeAtual.nome}</h3>
                        <p className="text-sm text-muted-foreground">
                          {jogador.timeAtual.tier && `Tier ${jogador.timeAtual.tier}`} • 
                          {jogador.timeAtual.contratando ? ' Contratando' : ' Não contratando'}
                        </p>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              )}

              {/* Histórico de Performance */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <TrendingUp className="h-5 w-5" />
                    Histórico de Performance
                  </CardTitle>
                  <CardDescription>
                    Sua evolução nas últimas partidas
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Trophy className="h-4 w-4 text-warning" />
                        <span className="text-sm text-muted-foreground">Vitórias (30 dias)</span>
                      </div>
                      <span className="font-semibold text-success">24</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Target className="h-4 w-4 text-primary" />
                        <span className="text-sm text-muted-foreground">Precisão média</span>
                      </div>
                      <span className="font-semibold text-foreground">68.5%</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Award className="h-4 w-4 text-secondary" />
                        <span className="text-sm text-muted-foreground">MVP's</span>
                      </div>
                      <span className="font-semibold text-foreground">12</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <Clock className="h-4 w-4 text-muted-foreground" />
                        <span className="text-sm text-muted-foreground">Tempo médio por partida</span>
                      </div>
                      <span className="font-semibold text-foreground">32min</span>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Conquistas e Badges */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Star className="h-5 w-5" />
                    Conquistas
                  </CardTitle>
                  <CardDescription>
                    Badges e conquistas desbloqueadas
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
                    <div className="flex flex-col items-center p-3 bg-muted/50 rounded-lg">
                      <Trophy className="h-6 w-6 text-warning mb-2" />
                      <span className="text-xs text-center font-medium">Primeira Vitória</span>
                    </div>
                    <div className="flex flex-col items-center p-3 bg-muted/50 rounded-lg">
                      <Target className="h-6 w-6 text-primary mb-2" />
                      <span className="text-xs text-center font-medium">Precisão 70%+</span>
                    </div>
                    <div className="flex flex-col items-center p-3 bg-muted/50 rounded-lg">
                      <Award className="h-6 w-6 text-secondary mb-2" />
                      <span className="text-xs text-center font-medium">5 MVP's</span>
                    </div>
                    <div className="flex flex-col items-center p-3 bg-muted/50 rounded-lg">
                      <Users className="h-6 w-6 text-success mb-2" />
                      <span className="text-xs text-center font-medium">Time Player</span>
                    </div>
                    <div className="flex flex-col items-center p-3 bg-muted/50 rounded-lg">
                      <TrendingUp className="h-6 w-6 text-warning mb-2" />
                      <span className="text-xs text-center font-medium">Em Ascensão</span>
                    </div>
                    <div className="flex flex-col items-center p-3 bg-muted/50 rounded-lg">
                      <Calendar className="h-6 w-6 text-primary mb-2" />
                      <span className="text-xs text-center font-medium">30 Dias Ativo</span>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Informações de Contato */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <MapPin className="h-5 w-5" />
                    Informações de Contato
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div className="flex items-center justify-between">
                      <span className="text-sm text-muted-foreground">Localização</span>
                      <span className="text-sm font-medium text-foreground">🇧🇷 São Paulo, SP</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-sm text-muted-foreground">Fuso horário</span>
                      <span className="text-sm font-medium text-foreground">GMT-3 (BRT)</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-sm text-muted-foreground">Idiomas</span>
                      <span className="text-sm font-medium text-foreground">Português, Inglês</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span className="text-sm text-muted-foreground">Disponibilidade</span>
                      <span className="text-sm font-medium text-success">Online agora</span>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Sidebar */}
            <div className="space-y-6">
              {/* Ações Rápidas */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle>Ações</CardTitle>
                </CardHeader>
                <CardContent className="space-y-3">
                  <Button 
                    className="w-full"
                    onClick={() => setEditandoPerfil(true)}
                  >
                    <Edit className="h-4 w-4 mr-2" />
                    Editar Perfil
                  </Button>
                  <Button variant="outline" className="w-full">
                    <BarChart3 className="h-4 w-4 mr-2" />
                    Ver Estatísticas
                  </Button>
                  <Button variant="outline" className="w-full">
                    <Search className="h-4 w-4 mr-2" />
                    Buscar Times
                  </Button>
                </CardContent>
              </Card>

              {/* Status da Conta */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle>Status da Conta</CardTitle>
                </CardHeader>
                <CardContent className="space-y-3">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Tipo de Usuário</span>
                    <span className="font-medium text-foreground">Jogador</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Plano</span>
                    <span className="font-medium text-foreground">{assinatura?.plano || 'Gratuito'}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Status</span>
                    <span className={`font-medium ${
                      assinatura?.status === 'Ativa' ? 'text-success' : 'text-warning'
                    }`}>
                      {assinatura?.status || 'Inativa'}
                    </span>
                  </div>
                  {assinatura?.dataFim && (
                    <div className="flex justify-between">
                      <span className="text-muted-foreground">Válido até</span>
                      <span className="font-medium text-foreground">
                        {new Date(assinatura.dataFim).toLocaleDateString('pt-BR')}
                      </span>
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Valor de Mercado */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <DollarSign className="h-5 w-5" />
                    Valor de Mercado
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-center">
                    <div className="text-3xl font-bold text-warning mb-2">
                      {jogador.valorDeMercado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                    </div>
                    <div className="flex items-center justify-center gap-2 text-sm text-muted-foreground">
                      <TrendingUp className="h-4 w-4 text-success" />
                      <span>+12% este mês</span>
                    </div>
                    <div className="mt-4 space-y-2">
                      <div className="flex justify-between text-sm">
                        <span className="text-muted-foreground">Última avaliação</span>
                        <span className="text-foreground">15/01/2024</span>
                      </div>
                      <div className="flex justify-between text-sm">
                        <span className="text-muted-foreground">Próxima avaliação</span>
                        <span className="text-foreground">15/02/2024</span>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Atividade Recente */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Clock className="h-5 w-5" />
                    Atividade Recente
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div className="flex items-center gap-3">
                      <div className="w-2 h-2 bg-success rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Partida vencida</p>
                        <p className="text-xs text-muted-foreground">2 horas atrás</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className="w-2 h-2 bg-primary rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Perfil atualizado</p>
                        <p className="text-xs text-muted-foreground">1 dia atrás</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className="w-2 h-2 bg-warning rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Nova conquista</p>
                        <p className="text-xs text-muted-foreground">3 dias atrás</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className="w-2 h-2 bg-secondary rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Time atualizado</p>
                        <p className="text-xs text-muted-foreground">1 semana atrás</p>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Metas e Objetivos */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Target className="h-5 w-5" />
                    Metas do Mês
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    <div>
                      <div className="flex justify-between text-sm mb-1">
                        <span className="text-muted-foreground">Partidas jogadas</span>
                        <span className="text-foreground">45/50</span>
                      </div>
                      <div className="w-full bg-muted rounded-full h-2">
                        <div className="bg-primary h-2 rounded-full" style={{ width: '90%' }}></div>
                      </div>
                    </div>
                    <div>
                      <div className="flex justify-between text-sm mb-1">
                        <span className="text-muted-foreground">Rating alvo</span>
                        <span className="text-foreground">2.1k/2.5k</span>
                      </div>
                      <div className="w-full bg-muted rounded-full h-2">
                        <div className="bg-success h-2 rounded-full" style={{ width: '84%' }}></div>
                      </div>
                    </div>
                    <div>
                      <div className="flex justify-between text-sm mb-1">
                        <span className="text-muted-foreground">MVP's</span>
                        <span className="text-foreground">8/10</span>
                      </div>
                      <div className="w-full bg-muted rounded-full h-2">
                        <div className="bg-warning h-2 rounded-full" style={{ width: '80%' }}></div>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </main>

      {/* Modal de Edição de Perfil */}
      {editandoPerfil && jogador && (
        <EditarPerfilJogador
          jogador={jogador}
          onClose={() => setEditandoPerfil(false)}
          onSuccess={handlePerfilAtualizado}
        />
      )}
    </div>
  );
};

export default PerfilJogador;
