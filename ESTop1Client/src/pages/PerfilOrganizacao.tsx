import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Time, Jogador } from '../types';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { Loader2, Building2, Edit, UserPlus, Search, BarChart3, CheckCircle, AlertCircle, Users, Trophy, Target, TrendingUp, Calendar, MapPin, DollarSign, Award, Clock, Star, Activity, Zap, Shield, Globe } from 'lucide-react';
import EditarPerfilTime from '../components/EditarPerfilTime';

const PerfilOrganizacao: React.FC = () => {
  const { user, assinatura } = useAuth();
  const [time, setTime] = useState<Time | null>(null);
  const [jogadores, setJogadores] = useState<Jogador[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [editandoTime, setEditandoTime] = useState(false);

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

  if (!time) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <Card className="w-full max-w-md animate-fade-in">
          <CardHeader className="text-center">
            <div className="flex justify-center mb-4">
              <div className="p-4 rounded-full bg-muted">
                <Building2 className="h-12 w-12 text-muted-foreground" />
              </div>
            </div>
            <CardTitle className="text-2xl">Time não encontrado</CardTitle>
            <CardDescription>
              Parece que você ainda não tem um time criado. 
              Crie seu time para começar a gerenciar jogadores.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button 
              onClick={() => window.location.href = '/times'}
              className="w-full"
            >
              Criar Time
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  const jogadoresContratados = jogadores.filter(j => j.disponibilidade === 'EmTime');
  const jogadoresLivres = jogadores.filter(j => j.disponibilidade === 'Livre');

  const handleTimeAtualizado = (timeAtualizado: Time) => {
    setTime(timeAtualizado);
    setJogadores(timeAtualizado.jogadores || []);
  };

  return (
    <div className="min-h-screen bg-background">
      <main className="container mx-auto px-4 py-8">
        <div className="max-w-6xl mx-auto">
          {/* Header do Perfil */}
          <Card className="mb-8 animate-fade-in">
            <CardContent className="p-8">
              <div className="flex flex-col sm:flex-row items-start sm:items-center space-y-4 sm:space-y-0 sm:space-x-6">
                {/* Logo do Time */}
                <div className="flex-shrink-0">
                  <div className="w-24 h-24 bg-gradient-to-br from-primary to-secondary rounded-full flex items-center justify-center text-white text-2xl font-bold shadow-lg">
                    {time.nome.charAt(0)}
                  </div>
                </div>
                
                {/* Informações Básicas */}
                <div className="flex-1 min-w-0">
                  <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                    <div>
                      <h1 className="text-3xl font-bold mb-2">
                        {time.nome}
                      </h1>
                      <div className="flex flex-wrap items-center gap-4 text-sm text-muted-foreground">
                        <span className="flex items-center">
                          <span className="w-2 h-2 bg-primary rounded-full mr-2"></span>
                          {time.tier ? `Tier ${time.tier}` : 'Sem tier'}
                        </span>
                        <span className="flex items-center">
                          <span className={`w-2 h-2 rounded-full mr-2 ${
                            time.contratando ? 'bg-success' : 'bg-destructive'
                          }`}></span>
                          {time.contratando ? 'Contratando' : 'Não contratando'}
                        </span>
                        <span>🇧🇷 {time.pais}</span>
                        <span>{jogadores.length} jogadores</span>
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
                        onClick={() => setEditandoTime(true)}
                        size="sm"
                        className="w-full sm:w-auto"
                      >
                        <Edit className="h-4 w-4 mr-2" />
                        Editar Time
                      </Button>
                    </div>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
            {/* Conteúdo Principal */}
            <div className="lg:col-span-3 space-y-6">
              {/* Estatísticas do Time */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <BarChart3 className="h-5 w-5" />
                    Estatísticas do Time
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div className="text-center">
                      <div className="text-2xl font-bold text-primary">{jogadores.length}</div>
                      <div className="text-sm text-muted-foreground">Total de Jogadores</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold text-success">{jogadoresContratados.length}</div>
                      <div className="text-sm text-muted-foreground">Contratados</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold text-warning">{jogadoresLivres.length}</div>
                      <div className="text-sm text-muted-foreground">Em Teste</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold text-secondary">
                        {jogadores.reduce((acc, jogador) => acc + jogador.valorDeMercado, 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                      </div>
                      <div className="text-sm text-muted-foreground">Valor Total</div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Lista de Jogadores */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <div className="flex items-center justify-between">
                    <CardTitle className="flex items-center gap-2">
                      <Users className="h-5 w-5" />
                      Jogadores
                    </CardTitle>
                    <Button size="sm">
                      <UserPlus className="h-4 w-4 mr-2" />
                      Adicionar Jogador
                    </Button>
                  </div>
                </CardHeader>
                <CardContent>
                  {jogadores.length > 0 ? (
                    <div className="space-y-3">
                      {jogadores.map((jogador) => (
                        <div key={jogador.id} className="flex items-center justify-between p-4 bg-muted/50 rounded-lg">
                          <div className="flex items-center space-x-4">
                            <img
                              src={jogador.fotoUrl || '/player-default.jpg'}
                              alt={jogador.apelido}
                              className="w-12 h-12 rounded-full object-cover"
                            />
                            <div>
                              <h3 className="font-semibold text-foreground">{jogador.apelido}</h3>
                              <p className="text-sm text-muted-foreground">
                                {jogador.funcaoPrincipal} • {jogador.status}
                              </p>
                            </div>
                          </div>
                          <div className="flex items-center space-x-4">
                            <div className="text-right">
                              <div className="font-semibold text-foreground">
                                {jogador.valorDeMercado.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                              </div>
                              <div className={`text-sm ${
                                jogador.disponibilidade === 'EmTime' ? 'text-success' : 'text-warning'
                              }`}>
                                {jogador.disponibilidade}
                              </div>
                            </div>
                            <Button variant="ghost" size="sm">
                              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z" />
                              </svg>
                            </Button>
                          </div>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <div className="text-center py-8">
                      <Users className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
                      <p className="text-muted-foreground mb-4">Nenhum jogador cadastrado</p>
                      <Button>
                        <UserPlus className="h-4 w-4 mr-2" />
                        Adicionar Primeiro Jogador
                      </Button>
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Informações do Time */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Building2 className="h-5 w-5" />
                    Informações do Time
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">Nome</label>
                      <p className="text-foreground">{time.nome}</p>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">País</label>
                      <p className="text-foreground">🇧🇷 {time.pais}</p>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">Tier</label>
                      <p className="text-foreground">{time.tier ? `Tier ${time.tier}` : 'Não definido'}</p>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-muted-foreground mb-1">Status de Contratação</label>
                      <p className="text-foreground">{time.contratando ? 'Contratando' : 'Não contratando'}</p>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Performance do Time */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Activity className="h-5 w-5" />
                    Performance do Time
                  </CardTitle>
                  <CardDescription>
                    Estatísticas de performance dos últimos 30 dias
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div className="text-center">
                      <div className="text-2xl font-bold text-success">78%</div>
                      <div className="text-sm text-muted-foreground">Taxa de Vitória</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold text-primary">2.1k</div>
                      <div className="text-sm text-muted-foreground">Rating Médio</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold text-warning">156</div>
                      <div className="text-sm text-muted-foreground">Partidas Jogadas</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold text-secondary">1.4</div>
                      <div className="text-sm text-muted-foreground">K/D Médio</div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Conquistas do Time */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Trophy className="h-5 w-5" />
                    Conquistas do Time
                  </CardTitle>
                  <CardDescription>
                    Títulos e conquistas alcançadas
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="flex items-center gap-3 p-3 bg-muted/50 rounded-lg">
                      <Trophy className="h-8 w-8 text-warning" />
                      <div>
                        <p className="font-medium text-foreground">Campeonato Regional</p>
                        <p className="text-sm text-muted-foreground">1º lugar - Janeiro 2024</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3 p-3 bg-muted/50 rounded-lg">
                      <Award className="h-8 w-8 text-primary" />
                      <div>
                        <p className="font-medium text-foreground">Melhor Time</p>
                        <p className="text-sm text-muted-foreground">Prêmio da Comunidade</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3 p-3 bg-muted/50 rounded-lg">
                      <Star className="h-8 w-8 text-secondary" />
                      <div>
                        <p className="font-medium text-foreground">Time em Ascensão</p>
                        <p className="text-sm text-muted-foreground">Crescimento 300%</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3 p-3 bg-muted/50 rounded-lg">
                      <Target className="h-8 w-8 text-success" />
                      <div>
                        <p className="font-medium text-foreground">Meta Mensal</p>
                        <p className="text-sm text-muted-foreground">100 partidas - Concluída</p>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Recursos e Ferramentas */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Zap className="h-5 w-5" />
                    Recursos Disponíveis
                  </CardTitle>
                  <CardDescription>
                    Ferramentas e recursos do seu plano
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="flex items-center gap-3">
                      <div className={`p-2 rounded-lg ${assinatura?.plano !== 'Gratuito' ? 'bg-success/20' : 'bg-muted'}`}>
                        <Search className={`h-4 w-4 ${assinatura?.plano !== 'Gratuito' ? 'text-success' : 'text-muted-foreground'}`} />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-foreground">Busca com IA</p>
                        <p className="text-xs text-muted-foreground">
                          {assinatura?.plano !== 'Gratuito' ? 'Disponível' : 'Bloqueado'}
                        </p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className={`p-2 rounded-lg ${assinatura?.plano === 'Trimestral' ? 'bg-success/20' : 'bg-muted'}`}>
                        <Shield className={`h-4 w-4 ${assinatura?.plano === 'Trimestral' ? 'text-success' : 'text-muted-foreground'}`} />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-foreground">API Access</p>
                        <p className="text-xs text-muted-foreground">
                          {assinatura?.plano === 'Trimestral' ? 'Disponível' : 'Bloqueado'}
                        </p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className="p-2 rounded-lg bg-success/20">
                        <BarChart3 className="h-4 w-4 text-success" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-foreground">Estatísticas Avançadas</p>
                        <p className="text-xs text-muted-foreground">Disponível</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className="p-2 rounded-lg bg-success/20">
                        <Globe className="h-4 w-4 text-success" />
                      </div>
                      <div>
                        <p className="text-sm font-medium text-foreground">Integração FACEIT</p>
                        <p className="text-xs text-muted-foreground">Disponível</p>
                      </div>
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
                    onClick={() => setEditandoTime(true)}
                  >
                    <Edit className="h-4 w-4 mr-2" />
                    Editar Time
                  </Button>
                  <Button variant="outline" className="w-full">
                    <UserPlus className="h-4 w-4 mr-2" />
                    Adicionar Jogador
                  </Button>
                  <Button variant="outline" className="w-full">
                    <Search className="h-4 w-4 mr-2" />
                    Buscar Jogadores
                  </Button>
                  <Button variant="outline" className="w-full">
                    <BarChart3 className="h-4 w-4 mr-2" />
                    Estatísticas
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
                    <span className="font-medium text-foreground">Organização</span>
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

              {/* Limites do Plano */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle>Limites do Plano</CardTitle>
                </CardHeader>
                <CardContent className="space-y-3">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Jogadores</span>
                    <span className="font-medium text-foreground">
                      {jogadores.length} / {assinatura?.plano === 'Trimestral' ? '∞' : '10'}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Busca IA</span>
                    <span className={`font-medium ${
                      assinatura?.plano !== 'Gratuito' ? 'text-success' : 'text-destructive'
                    }`}>
                      {assinatura?.plano !== 'Gratuito' ? 'Disponível' : 'Bloqueado'}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">API</span>
                    <span className={`font-medium ${
                      assinatura?.plano === 'Trimestral' ? 'text-success' : 'text-destructive'
                    }`}>
                      {assinatura?.plano === 'Trimestral' ? 'Disponível' : 'Bloqueado'}
                    </span>
                  </div>
                </CardContent>
              </Card>

              {/* Valor Total do Time */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <DollarSign className="h-5 w-5" />
                    Valor Total do Time
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-center">
                    <div className="text-3xl font-bold text-warning mb-2">
                      {jogadores.reduce((acc, jogador) => acc + jogador.valorDeMercado, 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                    </div>
                    <div className="flex items-center justify-center gap-2 text-sm text-muted-foreground">
                      <TrendingUp className="h-4 w-4 text-success" />
                      <span>+8% este mês</span>
                    </div>
                    <div className="mt-4 space-y-2">
                      <div className="flex justify-between text-sm">
                        <span className="text-muted-foreground">Valor médio por jogador</span>
                        <span className="text-foreground">
                          {jogadores.length > 0 ? (jogadores.reduce((acc, jogador) => acc + jogador.valorDeMercado, 0) / jogadores.length).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) : 'R$ 0,00'}
                        </span>
                      </div>
                      <div className="flex justify-between text-sm">
                        <span className="text-muted-foreground">Jogador mais valioso</span>
                        <span className="text-foreground">
                          {jogadores.length > 0 ? Math.max(...jogadores.map(j => j.valorDeMercado)).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) : 'R$ 0,00'}
                        </span>
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
                        <p className="text-sm font-medium text-foreground">Novo jogador contratado</p>
                        <p className="text-xs text-muted-foreground">1 hora atrás</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className="w-2 h-2 bg-primary rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Time atualizado</p>
                        <p className="text-xs text-muted-foreground">3 horas atrás</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className="w-2 h-2 bg-warning rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Nova conquista</p>
                        <p className="text-xs text-muted-foreground">1 dia atrás</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3">
                      <div className="w-2 h-2 bg-secondary rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Estatísticas atualizadas</p>
                        <p className="text-xs text-muted-foreground">2 dias atrás</p>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Metas do Time */}
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
                        <span className="text-muted-foreground">Partidas do time</span>
                        <span className="text-foreground">89/100</span>
                      </div>
                      <div className="w-full bg-muted rounded-full h-2">
                        <div className="bg-primary h-2 rounded-full" style={{ width: '89%' }}></div>
                      </div>
                    </div>
                    <div>
                      <div className="flex justify-between text-sm mb-1">
                        <span className="text-muted-foreground">Taxa de vitória</span>
                        <span className="text-foreground">78%/80%</span>
                      </div>
                      <div className="w-full bg-muted rounded-full h-2">
                        <div className="bg-success h-2 rounded-full" style={{ width: '97%' }}></div>
                      </div>
                    </div>
                    <div>
                      <div className="flex justify-between text-sm mb-1">
                        <span className="text-muted-foreground">Novos jogadores</span>
                        <span className="text-foreground">2/3</span>
                      </div>
                      <div className="w-full bg-muted rounded-full h-2">
                        <div className="bg-warning h-2 rounded-full" style={{ width: '67%' }}></div>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Próximos Eventos */}
              <Card className="animate-fade-in">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Calendar className="h-5 w-5" />
                    Próximos Eventos
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    <div className="flex items-center gap-3 p-2 bg-muted/50 rounded-lg">
                      <div className="w-2 h-2 bg-primary rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Campeonato Regional</p>
                        <p className="text-xs text-muted-foreground">15/02/2024 - 20:00</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3 p-2 bg-muted/50 rounded-lg">
                      <div className="w-2 h-2 bg-success rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Treino de Equipe</p>
                        <p className="text-xs text-muted-foreground">18/02/2024 - 19:00</p>
                      </div>
                    </div>
                    <div className="flex items-center gap-3 p-2 bg-muted/50 rounded-lg">
                      <div className="w-2 h-2 bg-warning rounded-full"></div>
                      <div className="flex-1">
                        <p className="text-sm font-medium text-foreground">Avaliação Mensal</p>
                        <p className="text-xs text-muted-foreground">28/02/2024</p>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </main>

      {/* Modal de Edição de Time */}
      {editandoTime && time && (
        <EditarPerfilTime
          time={time}
          onClose={() => setEditandoTime(false)}
          onSuccess={handleTimeAtualizado}
        />
      )}
    </div>
  );
};

export default PerfilOrganizacao;
