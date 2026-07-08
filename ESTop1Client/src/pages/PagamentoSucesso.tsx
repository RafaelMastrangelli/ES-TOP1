import { useEffect, useState } from 'react';
import { useNavigate, useLocation, useSearchParams } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { 
  CheckCircle, 
  Crown, 
  Star, 
  Zap, 
  ArrowRight, 
  Download,
  Calendar,
  Users,
  Target,
  Loader2
} from 'lucide-react';

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
}

interface DadosPagamento {
  metodo: 'cartao' | 'pix' | 'boleto';
  numeroCartao?: string;
  nomeCartao?: string;
  validadeCartao?: string;
  cvvCartao?: string;
  cpf?: string;
  email?: string;
}

export default function PagamentoSucesso() {
  const navigate = useNavigate();
  const location = useLocation();
  const [searchParams] = useSearchParams();
  const { user } = useAuth();
  const [plano, setPlano] = useState<Plano | null>(null);
  const [dadosPagamento, setDadosPagamento] = useState<DadosPagamento | null>(null);
  const [pagamentoId, setPagamentoId] = useState<string | null>(null);
  const [dataInicio, setDataInicio] = useState<Date>(new Date());
  const [dataFim, setDataFim] = useState<Date>(new Date());
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const calcularDatas = (tipoPlano: string) => {
    const inicio = new Date();
    const fim = new Date();

    if (tipoPlano === 'Trimestral') {
      fim.setMonth(fim.getMonth() + 3);
    } else if (tipoPlano === 'Enterprise') {
      fim.setMonth(fim.getMonth() + 12);
    } else {
      fim.setMonth(fim.getMonth() + 1);
    }

    setDataInicio(inicio);
    setDataFim(fim);
  };

  const montarPlanoAPartirDoStatus = (tipo: string, valor: number): Plano => ({
    id: tipo,
    tipo,
    nome: tipo,
    descricao: `Plano ${tipo}`,
    valorMensal: valor,
    duracao: tipo === 'Trimestral' ? '3 meses' : tipo === 'Enterprise' ? '12 meses' : '1 mês',
    limiteJogadores: tipo === 'Gratuito' ? 5 : tipo === 'Mensal' ? 50 : -1,
    acessoEstatisticas: true,
    acessoBuscaIA: tipo !== 'Gratuito',
    acessoAPI: tipo === 'Trimestral' || tipo === 'Enterprise',
    suportePrioritario: tipo === 'Trimestral' || tipo === 'Enterprise',
  });

  useEffect(() => {
    const planoData = location.state?.plano as Plano | undefined;
    const pagamentoData = location.state?.dadosPagamento as DadosPagamento | undefined;
    const pagamentoStateId = location.state?.pagamentoId as string | undefined;
    const pagamentoQueryId = searchParams.get('pagamentoId');
    const idPagamento = pagamentoStateId || pagamentoQueryId;

    if (planoData) {
      setPlano(planoData);
      setDadosPagamento(pagamentoData || null);
      setPagamentoId(idPagamento || null);
      calcularDatas(planoData.tipo);
      setCarregando(false);
      return;
    }

    if (!idPagamento) {
      navigate('/assinaturas');
      return;
    }

    setPagamentoId(idPagamento);

    const carregarStatus = async () => {
      try {
        const status = await api.pagamentos.obterStatus(idPagamento);

        if (status.status !== 'Aprovado') {
          const pending = searchParams.get('pending');
          if (pending) {
            setErro('Pagamento ainda pendente. Aguarde a confirmação ou tente novamente.');
          } else {
            setErro('Pagamento não confirmado. Verifique o status em Assinaturas.');
          }
          return;
        }

        const planoMontado = montarPlanoAPartirDoStatus(status.plano, status.valor);
        setPlano(planoMontado);
        calcularDatas(status.plano);
      } catch {
        setErro('Não foi possível verificar o pagamento.');
      } finally {
        setCarregando(false);
      }
    };

    carregarStatus();
  }, [location.state, navigate, searchParams]);

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

  const formatarData = (data: Date) => {
    return data.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  };

  const gerarComprovante = () => {
    if (!plano) return;

    const metodoPagamento = dadosPagamento?.metodo
      ? dadosPagamento.metodo === 'cartao'
        ? 'Cartão de Crédito'
        : dadosPagamento.metodo === 'pix'
          ? 'PIX'
          : 'Boleto Bancário'
      : 'Mercado Pago';

    const comprovante = `
COMPROVANTE DE ASSINATURA - ES-TOP1
=====================================

Data: ${formatarData(new Date())}
Cliente: ${user?.nome || 'Usuário'}
E-mail: ${dadosPagamento?.email || user?.email || '-'}
${pagamentoId ? `ID Pagamento: ${pagamentoId}\n` : ''}
PLANO CONTRATADO:
- Nome: ${plano.nome}
- Descrição: ${plano.descricao}
- Valor: R$ ${plano.valorMensal.toFixed(2)}
- Duração: ${plano.duracao}
- Período: ${formatarData(dataInicio)} a ${formatarData(dataFim)}

MÉTODO DE PAGAMENTO:
- ${metodoPagamento}

RECURSOS INCLUSOS:
- ${plano.limiteJogadores === -1 ? 'Jogadores ilimitados' : `${plano.limiteJogadores} jogadores`}
- Estatísticas detalhadas
${plano.acessoBuscaIA ? '- Busca com IA\n' : ''}${plano.acessoAPI ? '- Acesso à API\n' : ''}${plano.suportePrioritario ? '- Suporte prioritário\n' : ''}

Obrigado por escolher a ES-TOP1!
    `.trim();

    const blob = new Blob([comprovante], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `comprovante-assinatura-${plano.tipo.toLowerCase()}-${new Date().toISOString().split('T')[0]}.txt`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  if (carregando) {
    return (
      <div className="min-h-screen bg-background">
        <main className="container mx-auto px-4 py-8">
          <div className="flex items-center justify-center h-64">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
        </main>
      </div>
    );
  }

  if (erro || !plano) {
    return (
      <div className="min-h-screen bg-background">
        <main className="container mx-auto px-4 py-8">
          <div className="max-w-lg mx-auto text-center space-y-4">
            <p className="text-muted-foreground">{erro || 'Não foi possível carregar os dados do pagamento.'}</p>
            <Button onClick={() => navigate('/assinaturas')}>Voltar para Assinaturas</Button>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      
      <main className="container mx-auto px-4 py-8">
        <div className="max-w-4xl mx-auto">
          {/* Header de Sucesso */}
          <div className="text-center mb-12">
            <div className="flex justify-center mb-6">
              <div className="p-4 rounded-full bg-success/10">
                <CheckCircle className="h-16 w-16 text-success" />
              </div>
            </div>
            <h1 className="text-4xl md:text-5xl font-bold mb-4 text-success">
              Pagamento Realizado com Sucesso!
            </h1>
            <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
              Sua assinatura foi ativada e você já pode aproveitar todos os recursos da ES-TOP1
            </p>
          </div>

          <div className="grid lg:grid-cols-2 gap-8 mb-8">
            {/* Detalhes da Assinatura */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  {getPlanoIcon(plano.tipo)}
                  Detalhes da Assinatura
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <Badge className={`${getPlanoColor(plano.tipo)} mb-2`}>
                    {plano.nome}
                  </Badge>
                  <p className="text-sm text-muted-foreground">{plano.descricao}</p>
                </div>
                
                <div className="space-y-3">
                  <div className="flex items-center gap-3">
                    <Calendar className="h-4 w-4 text-muted-foreground" />
                    <div>
                      <div className="text-sm font-medium">Período de Vigência</div>
                      <div className="text-sm text-muted-foreground">
                        {formatarData(dataInicio)} - {formatarData(dataFim)}
                      </div>
                    </div>
                  </div>
                  
                  <div className="flex items-center gap-3">
                    <Target className="h-4 w-4 text-muted-foreground" />
                    <div>
                      <div className="text-sm font-medium">Valor Pago</div>
                      <div className="text-sm text-muted-foreground">
                        R$ {plano.valorMensal.toFixed(2)}
                        {plano.tipo === 'Trimestral' ? '/trimestre' : '/mês'}
                      </div>
                    </div>
                  </div>
                  
                  <div className="flex items-center gap-3">
                    <Users className="h-4 w-4 text-muted-foreground" />
                    <div>
                      <div className="text-sm font-medium">Limite de Jogadores</div>
                      <div className="text-sm text-muted-foreground">
                        {plano.limiteJogadores === -1 ? 'Ilimitado' : `${plano.limiteJogadores} jogadores`}
                      </div>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Recursos Inclusos */}
            <Card>
              <CardHeader>
                <CardTitle>Recursos Inclusos</CardTitle>
                <CardDescription>
                  Todos estes recursos estão disponíveis em sua conta
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-3">
                  <div className="flex items-center gap-3">
                    <CheckCircle className="h-4 w-4 text-success" />
                    <span className="text-sm">Estatísticas detalhadas</span>
                  </div>
                  
                  {plano.acessoBuscaIA && (
                    <div className="flex items-center gap-3">
                      <CheckCircle className="h-4 w-4 text-success" />
                      <span className="text-sm">Busca com IA</span>
                    </div>
                  )}
                  
                  {plano.acessoAPI && (
                    <div className="flex items-center gap-3">
                      <CheckCircle className="h-4 w-4 text-success" />
                      <span className="text-sm">Acesso à API</span>
                    </div>
                  )}
                  
                  {plano.suportePrioritario && (
                    <div className="flex items-center gap-3">
                      <CheckCircle className="h-4 w-4 text-success" />
                      <span className="text-sm">Suporte prioritário</span>
                    </div>
                  )}
                  
                  <div className="flex items-center gap-3">
                    <CheckCircle className="h-4 w-4 text-success" />
                    <span className="text-sm">
                      {plano.limiteJogadores === -1 ? 'Jogadores ilimitados' : `${plano.limiteJogadores} jogadores`}
                    </span>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Ações */}
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button 
              onClick={() => navigate('/jogadores')}
              className="flex items-center gap-2"
            >
              <Target className="h-4 w-4" />
              Explorar Jogadores
              <ArrowRight className="h-4 w-4" />
            </Button>
            
            <Button 
              variant="outline"
              onClick={gerarComprovante}
              className="flex items-center gap-2"
            >
              <Download className="h-4 w-4" />
              Baixar Comprovante
            </Button>
            
            <Button 
              variant="outline"
              onClick={() => navigate('/assinaturas')}
            >
              Gerenciar Assinatura
            </Button>
          </div>

          {/* Informações Adicionais */}
          <Card className="mt-8">
            <CardContent className="pt-6">
              <div className="text-center space-y-2">
                <h3 className="font-medium">Próximos Passos</h3>
                <p className="text-sm text-muted-foreground">
                  Sua assinatura está ativa! Você pode começar a explorar jogadores, 
                  criar times e aproveitar todos os recursos da plataforma.
                </p>
                <p className="text-sm text-muted-foreground">
                  Em caso de dúvidas, entre em contato conosco através do suporte.
                </p>
              </div>
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  );
}
