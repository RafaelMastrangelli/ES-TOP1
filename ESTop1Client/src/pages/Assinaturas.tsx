import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { Loader2, Check, X, Crown, Star, Zap, Target, Users } from 'lucide-react';
import Navbar from '../components/Navbar';

interface Plano {
  id: string;
  tipo: string;
  nome: string;
  descricao: string;
  valorMensal: number;
  duracao: string;
  limiteJogadores: number;
  acessoEstatisticas: boolean;
  acessoBuscaIA: boolean;
  acessoAPI: boolean;
  suportePrioritario: boolean;
  popular?: boolean;
}

interface Assinatura {
  id: string;
  plano: string;
  status: string;
  dataInicio: string;
  dataFim: string;
  valorMensal: number;
}

export default function Assinaturas() {
  const navigate = useNavigate();
  const { user, assinatura } = useAuth();
  const [planos, setPlanos] = useState<Plano[]>([]);
  const [minhaAssinatura, setMinhaAssinatura] = useState<Assinatura | null>(null);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  useEffect(() => {
    carregarDados();
  }, []);

  const carregarDados = async () => {
    try {
      // Planos fixos conforme especificado
      const planosFixos: Plano[] = [
        {
          id: '1',
          tipo: 'Gratuito',
          nome: 'Gratuito',
          descricao: 'Perfeito para começar e testar a plataforma',
          valorMensal: 0,
          duracao: '1 mês',
          limiteJogadores: 5,
          acessoEstatisticas: true,
          acessoBuscaIA: false,
          acessoAPI: false,
          suportePrioritario: false,
        },
        {
          id: '2',
          tipo: 'Mensal',
          nome: 'Mensal',
          descricao: 'Ideal para uso regular e descoberta de talentos',
          valorMensal: 100,
          duracao: '1 mês',
          limiteJogadores: 50,
          acessoEstatisticas: true,
          acessoBuscaIA: true,
          acessoAPI: false,
          suportePrioritario: false,
          popular: true,
        },
        {
          id: '3',
          tipo: 'Trimestral',
          nome: 'Trimestral',
          descricao: 'Melhor custo-benefício para times profissionais',
          valorMensal: 230,
          duracao: '3 meses',
          limiteJogadores: -1, // Ilimitado
          acessoEstatisticas: true,
          acessoBuscaIA: true,
          acessoAPI: true,
          suportePrioritario: true,
        }
      ];

      const assinaturaData = await api.assinaturas.obterMinha().catch(() => null);
      
      setPlanos(planosFixos);
      setMinhaAssinatura(assinaturaData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCriarAssinatura = async (planoSelecionado: Plano) => {
    if (planoSelecionado.tipo === 'Gratuito') {
      // Para plano gratuito, criar diretamente
      setActionLoading(planoSelecionado.tipo);
      try {
        await api.assinaturas.criar(planoSelecionado.tipo);
        await carregarDados();
      } catch (error) {
        console.error('Erro ao criar assinatura:', error);
      } finally {
        setActionLoading(null);
      }
    } else {
      // Para planos pagos, navegar para página de pagamento
      navigate('/pagamento', { state: { plano: planoSelecionado } });
    }
  };

  const handleCancelarAssinatura = async () => {
    if (!minhaAssinatura) return;
    
    setActionLoading('cancelar');
    try {
      await api.assinaturas.cancelar(minhaAssinatura.id);
      await carregarDados();
    } catch (error) {
      console.error('Erro ao cancelar assinatura:', error);
    } finally {
      setActionLoading(null);
    }
  };

  const getPlanoIcon = (tipo: string) => {
    switch (tipo) {
      case 'Gratuito': return <Star className="h-6 w-6" />;
      case 'Mensal': return <Zap className="h-6 w-6" />;
      case 'Trimestral': return <Crown className="h-6 w-6" />;
      default: return <Star className="h-6 w-6" />;
    }
  };

  const getPlanoColor = (tipo: string) => {
    switch (tipo) {
      case 'Gratuito': return 'bg-muted text-muted-foreground';
      case 'Mensal': return 'bg-secondary/20 text-secondary-foreground';
      case 'Trimestral': return 'bg-primary/20 text-primary-foreground';
      default: return 'bg-muted text-muted-foreground';
    }
  };

  const getPlanoGradient = (tipo: string) => {
    switch (tipo) {
      case 'Gratuito': return '';
      case 'Mensal': return 'bg-gradient-secondary';
      case 'Trimestral': return 'bg-gradient-primary';
      default: return '';
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-background">
        <Navbar />
        <main className="container mx-auto px-4 py-8">
          <div className="flex items-center justify-center h-64">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      
      <main className="container mx-auto px-4 py-8">
        <div className="max-w-7xl mx-auto">
          {/* Hero Section */}
          <div className="text-center mb-12 animate-fade-in">
            <div className="flex justify-center mb-6">
              <div className="p-4 rounded-full bg-primary/10">
                <Target className="h-12 w-12 text-primary" />
              </div>
            </div>
            <h1 className="text-4xl md:text-5xl font-bold mb-4">
              Planos de Assinatura
            </h1>
            <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
              Escolha o plano ideal para sua organização e tenha acesso completo à plataforma ES-TOP1
            </p>
          </div>

          {/* Assinatura Atual */}
          {minhaAssinatura && (
            <Card className="mb-12 border-success/20 bg-success/5 animate-fade-in">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-success">
                  <Check className="h-5 w-5" />
                  Sua Assinatura Atual
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex items-center justify-between">
                  <div>
                    <Badge className={`${getPlanoColor(minhaAssinatura.plano)} mb-2`}>
                      {minhaAssinatura.plano}
                    </Badge>
                    <p className="text-sm text-muted-foreground">
                      Válida até {new Date(minhaAssinatura.dataFim).toLocaleDateString('pt-BR')}
                    </p>
                    <p className="text-lg font-semibold">
                      R$ {minhaAssinatura.valorMensal.toFixed(2)}/mês
                    </p>
                  </div>
                  <Button
                    variant="outline"
                    onClick={handleCancelarAssinatura}
                    disabled={actionLoading === 'cancelar'}
                    className="text-destructive border-destructive/20 hover:bg-destructive/10"
                  >
                    {actionLoading === 'cancelar' ? (
                      <Loader2 className="h-4 w-4 animate-spin" />
                    ) : (
                      <X className="h-4 w-4" />
                    )}
                    Cancelar
                  </Button>
                </div>
              </CardContent>
            </Card>
          )}

          {/* Grid de Planos */}
          <div id="planos" className="grid md:grid-cols-3 gap-8 mb-16">
            {planos.map((plano, index) => {
              const isCurrentPlan = minhaAssinatura?.plano === plano.tipo;
              const isFree = plano.tipo === 'Gratuito';
              const isPopular = plano.popular;
              
              return (
                <Card 
                  key={plano.id} 
                  className={`relative group hover:shadow-lg transition-all duration-300 ${
                    isCurrentPlan ? 'ring-2 ring-success' : ''
                  } ${isPopular ? 'scale-105' : ''} animate-fade-in`}
                  style={{ animationDelay: `${index * 100}ms` }}
                >
                  {isCurrentPlan && (
                    <div className="absolute -top-3 left-1/2 transform -translate-x-1/2 z-10">
                      <Badge className="bg-success text-white">Atual</Badge>
                    </div>
                  )}
                  
                  {isPopular && !isCurrentPlan && (
                    <div className="absolute -top-3 left-1/2 transform -translate-x-1/2 z-10">
                      <Badge className="bg-primary text-white">Mais Popular</Badge>
                    </div>
                  )}
                  
                  <CardHeader className={`text-center ${getPlanoGradient(plano.tipo)} rounded-t-lg`}>
                    <div className="flex justify-center mb-4">
                      <div className={`p-3 rounded-full ${
                        plano.tipo === 'Gratuito' ? 'bg-muted' :
                        plano.tipo === 'Mensal' ? 'bg-secondary/20' :
                        plano.tipo === 'Trimestral' ? 'bg-primary/20' :
                        'bg-muted'
                      }`}>
                        {getPlanoIcon(plano.tipo)}
                      </div>
                    </div>
                    <CardTitle className="text-xl">{plano.nome}</CardTitle>
                    <CardDescription className="text-muted-foreground">{plano.descricao}</CardDescription>
                    <div className="mt-4">
                      <div className="flex items-baseline justify-center gap-1">
                        <span className="text-3xl font-bold">R$ {plano.valorMensal.toFixed(2)}</span>
                        <span className="text-muted-foreground">
                          {plano.tipo === 'Trimestral' ? '/trimestre' : '/mês'}
                        </span>
                      </div>
                      <p className="text-sm text-muted-foreground mt-1">
                        {plano.duracao}
                      </p>
                      {plano.tipo === 'Trimestral' && (
                        <p className="text-xs text-success mt-1">
                          Economia de R$ 70 comparado ao mensal
                        </p>
                      )}
                    </div>
                  </CardHeader>
                
                  <CardContent>
                    <ul className="space-y-3 mb-6">
                      <li className="flex items-center gap-3">
                        <div className="flex-shrink-0">
                          {plano.limiteJogadores === -1 ? (
                            <Check className="h-4 w-4 text-success" />
                          ) : (
                            <Users className="h-4 w-4 text-muted-foreground" />
                          )}
                        </div>
                        <span className="text-sm">
                          {plano.limiteJogadores === -1 ? 'Jogadores ilimitados' : `${plano.limiteJogadores} jogadores`}
                        </span>
                      </li>
                      <li className="flex items-center gap-3">
                        <div className="flex-shrink-0">
                          {plano.acessoEstatisticas ? (
                            <Check className="h-4 w-4 text-success" />
                          ) : (
                            <X className="h-4 w-4 text-muted-foreground" />
                          )}
                        </div>
                        <span className="text-sm">Estatísticas detalhadas</span>
                      </li>
                      <li className="flex items-center gap-3">
                        <div className="flex-shrink-0">
                          {plano.acessoBuscaIA ? (
                            <Check className="h-4 w-4 text-success" />
                          ) : (
                            <X className="h-4 w-4 text-muted-foreground" />
                          )}
                        </div>
                        <span className="text-sm">Busca com IA</span>
                      </li>
                      <li className="flex items-center gap-3">
                        <div className="flex-shrink-0">
                          {plano.acessoAPI ? (
                            <Check className="h-4 w-4 text-success" />
                          ) : (
                            <X className="h-4 w-4 text-muted-foreground" />
                          )}
                        </div>
                        <span className="text-sm">Acesso à API</span>
                      </li>
                      <li className="flex items-center gap-3">
                        <div className="flex-shrink-0">
                          {plano.suportePrioritario ? (
                            <Check className="h-4 w-4 text-success" />
                          ) : (
                            <X className="h-4 w-4 text-muted-foreground" />
                          )}
                        </div>
                        <span className="text-sm">Suporte prioritário</span>
                      </li>
                    </ul>
                    
                    {!isCurrentPlan && (
                      <Button
                        className="w-full group-hover:scale-105 transition-transform"
                        variant={isFree ? "outline" : isPopular ? "default" : "secondary"}
                        onClick={() => handleCriarAssinatura(plano)}
                        disabled={actionLoading === plano.tipo}
                      >
                        {actionLoading === plano.tipo ? (
                          <Loader2 className="h-4 w-4 animate-spin" />
                        ) : (
                          isFree ? 'Ativar Gratuito' : `Assinar ${plano.tipo}`
                        )}
                      </Button>
                    )}
                  </CardContent>
              </Card>
            );
          })}
        </div>

        </div>
      </main>
    </div>
  );
}
