import { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
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
  Target
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
  const { user } = useAuth();
  const [plano, setPlano] = useState<Plano | null>(null);
  const [dadosPagamento, setDadosPagamento] = useState<DadosPagamento | null>(null);
  const [dataInicio, setDataInicio] = useState<Date>(new Date());
  const [dataFim, setDataFim] = useState<Date>(new Date());

  useEffect(() => {
    // Recuperar dados da navegação
    const planoData = location.state?.plano;
    const pagamentoData = location.state?.dadosPagamento;
    
    if (planoData) {
      setPlano(planoData);
      setDadosPagamento(pagamentoData);
      
      // Calcular datas da assinatura
      const inicio = new Date();
      const fim = new Date();
      
      if (planoData.tipo === 'Trimestral') {
        fim.setMonth(fim.getMonth() + 3);
      } else {
        fim.setMonth(fim.getMonth() + 1);
      }
      
      setDataInicio(inicio);
      setDataFim(fim);
    } else {
      // Se não há dados, redirecionar para assinaturas
      navigate('/assinaturas');
    }
  }, [location.state, navigate]);

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
    if (!plano || !dadosPagamento) return;

    const comprovante = `
COMPROVANTE DE ASSINATURA - ES-TOP1
=====================================

Data: ${formatarData(new Date())}
Cliente: ${user?.nome || 'Usuário'}
E-mail: ${dadosPagamento.email}

PLANO CONTRATADO:
- Nome: ${plano.nome}
- Descrição: ${plano.descricao}
- Valor: R$ ${plano.valorMensal.toFixed(2)}
- Duração: ${plano.duracao}
- Período: ${formatarData(dataInicio)} a ${formatarData(dataFim)}

MÉTODO DE PAGAMENTO:
- ${dadosPagamento.metodo === 'cartao' ? 'Cartão de Crédito' : 
    dadosPagamento.metodo === 'pix' ? 'PIX' : 'Boleto Bancário'}

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

  if (!plano) {
    return (
      <div className="min-h-screen bg-background">
        <main className="container mx-auto px-4 py-8">
          <div className="flex items-center justify-center h-64">
            <div className="text-center">
              <div className="text-muted-foreground">Carregando...</div>
            </div>
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
