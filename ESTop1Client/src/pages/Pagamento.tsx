import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { api } from '../lib/api';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Separator } from '../components/ui/separator';
import { 
  Loader2, 
  Check, 
  CreditCard, 
  Smartphone, 
  ArrowLeft, 
  Shield, 
  Lock,
  Crown,
  Star,
  Zap
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

export default function Pagamento() {
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuth();
  const [plano, setPlano] = useState<Plano | null>(null);
  const [dadosPagamento, setDadosPagamento] = useState<DadosPagamento>({
    metodo: 'cartao',
    cpf: '',
    email: user?.email || ''
  });
  const [loading, setLoading] = useState(false);
  const [etapa, setEtapa] = useState<'metodo' | 'dados' | 'confirmacao'>('metodo');
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    // Recuperar dados do plano da navegação
    const planoData = location.state?.plano;
    if (planoData) {
      setPlano(planoData);
    } else {
      // Se não há dados do plano, redirecionar para assinaturas
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

  const formatarValorCartao = (valor: string) => {
    return valor.replace(/\D/g, '').replace(/(\d{4})(?=\d)/g, '$1 ');
  };

  const formatarValidadeCartao = (valor: string) => {
    return valor.replace(/\D/g, '').replace(/(\d{2})(\d{2})/, '$1/$2');
  };

  const formatarCPF = (valor: string) => {
    return valor.replace(/\D/g, '').replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  };

  const handleProximo = () => {
    if (etapa === 'metodo') {
      setEtapa('dados');
    } else if (etapa === 'dados') {
      setEtapa('confirmacao');
    }
  };

  const handleVoltar = () => {
    if (etapa === 'dados') {
      setEtapa('metodo');
    } else if (etapa === 'confirmacao') {
      setEtapa('dados');
    }
  };

  const handleFinalizarPagamento = async () => {
    if (!plano) return;

    setLoading(true);
    setErro(null);

    try {
      if (plano.tipo === 'Gratuito') {
        // Para plano gratuito, apenas criar a assinatura
        await api.assinaturas.criar(plano.tipo);
      } else {
        // Para planos pagos, simular processamento de pagamento
        await new Promise(resolve => setTimeout(resolve, 2000)); // Simular delay
        await api.assinaturas.criar(plano.tipo);
      }

      // Redirecionar para página de sucesso
      navigate('/pagamento-sucesso', { 
        state: { 
          plano: plano,
          dadosPagamento: dadosPagamento 
        } 
      });
    } catch (error) {
      console.error('Erro ao processar pagamento:', error);
      setErro('Erro ao processar pagamento. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  const validarDados = () => {
    if (dadosPagamento.metodo === 'cartao') {
      return dadosPagamento.numeroCartao && 
             dadosPagamento.nomeCartao && 
             dadosPagamento.validadeCartao && 
             dadosPagamento.cvvCartao &&
             dadosPagamento.cpf;
    }
    return dadosPagamento.cpf && dadosPagamento.email;
  };

  if (!plano) {
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

  return (
    <div className="min-h-screen bg-background">
      
      <main className="container mx-auto px-4 py-8">
        <div className="max-w-4xl mx-auto">
          {/* Header */}
          <div className="mb-8">
            <Button
              variant="ghost"
              onClick={() => navigate('/assinaturas')}
              className="mb-4"
            >
              <ArrowLeft className="h-4 w-4 mr-2" />
              Voltar aos Planos
            </Button>
            
            <div className="flex items-center gap-2 mb-2">
              <div className="flex gap-1">
                <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${
                  etapa === 'metodo' ? 'bg-primary text-primary-foreground' : 'bg-muted text-muted-foreground'
                }`}>
                  1
                </div>
                <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${
                  etapa === 'dados' ? 'bg-primary text-primary-foreground' : 
                  etapa === 'confirmacao' ? 'bg-primary text-primary-foreground' : 'bg-muted text-muted-foreground'
                }`}>
                  2
                </div>
                <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${
                  etapa === 'confirmacao' ? 'bg-primary text-primary-foreground' : 'bg-muted text-muted-foreground'
                }`}>
                  3
                </div>
              </div>
              <div className="text-sm text-muted-foreground">
                {etapa === 'metodo' && 'Método de Pagamento'}
                {etapa === 'dados' && 'Dados do Pagamento'}
                {etapa === 'confirmacao' && 'Confirmação'}
              </div>
            </div>
          </div>

          <div className="grid lg:grid-cols-3 gap-8">
            {/* Resumo do Plano */}
            <div className="lg:col-span-1">
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    {getPlanoIcon(plano.tipo)}
                    Resumo do Plano
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div>
                    <Badge className={`${getPlanoColor(plano.tipo)} mb-2`}>
                      {plano.nome}
                    </Badge>
                    <p className="text-sm text-muted-foreground">{plano.descricao}</p>
                  </div>
                  
                  <Separator />
                  
                  <div className="space-y-2">
                    <div className="flex justify-between">
                      <span>Valor:</span>
                      <span className="font-semibold">
                        R$ {plano.valorMensal.toFixed(2)}
                        {plano.tipo === 'Trimestral' ? '/trimestre' : '/mês'}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span>Duração:</span>
                      <span>{plano.duracao}</span>
                    </div>
                    {plano.tipo === 'Trimestral' && (
                      <div className="flex justify-between text-success">
                        <span>Economia:</span>
                        <span>R$ 70,00</span>
                      </div>
                    )}
                  </div>

                  <Separator />

                  <div className="space-y-2 text-sm">
                    <div className="flex items-center gap-2">
                      <Check className="h-4 w-4 text-success" />
                      <span>
                        {plano.limiteJogadores === -1 ? 'Jogadores ilimitados' : `${plano.limiteJogadores} jogadores`}
                      </span>
                    </div>
                    <div className="flex items-center gap-2">
                      <Check className="h-4 w-4 text-success" />
                      <span>Estatísticas detalhadas</span>
                    </div>
                    {plano.acessoBuscaIA && (
                      <div className="flex items-center gap-2">
                        <Check className="h-4 w-4 text-success" />
                        <span>Busca com IA</span>
                      </div>
                    )}
                    {plano.acessoAPI && (
                      <div className="flex items-center gap-2">
                        <Check className="h-4 w-4 text-success" />
                        <span>Acesso à API</span>
                      </div>
                    )}
                    {plano.suportePrioritario && (
                      <div className="flex items-center gap-2">
                        <Check className="h-4 w-4 text-success" />
                        <span>Suporte prioritário</span>
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Formulário de Pagamento */}
            <div className="lg:col-span-2">
              <Card>
                <CardHeader>
                  <CardTitle>
                    {etapa === 'metodo' && 'Escolha o Método de Pagamento'}
                    {etapa === 'dados' && 'Dados do Pagamento'}
                    {etapa === 'confirmacao' && 'Confirmação do Pagamento'}
                  </CardTitle>
                  <CardDescription>
                    {etapa === 'metodo' && 'Selecione como deseja pagar sua assinatura'}
                    {etapa === 'dados' && 'Preencha os dados necessários para o pagamento'}
                    {etapa === 'confirmacao' && 'Revise os dados antes de finalizar'}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  {erro && (
                    <div className="mb-4 p-3 bg-destructive/10 border border-destructive/20 rounded-md text-destructive text-sm">
                      {erro}
                    </div>
                  )}

                  {etapa === 'metodo' && (
                    <div className="space-y-4">
                      <div className="grid gap-4">
                        <div 
                          className={`p-4 border rounded-lg cursor-pointer transition-colors ${
                            dadosPagamento.metodo === 'cartao' ? 'border-primary bg-primary/5' : 'border-border hover:border-primary/50'
                          }`}
                          onClick={() => setDadosPagamento(prev => ({ ...prev, metodo: 'cartao' }))}
                        >
                          <div className="flex items-center gap-3">
                            <CreditCard className="h-5 w-5" />
                            <div>
                              <div className="font-medium">Cartão de Crédito</div>
                              <div className="text-sm text-muted-foreground">
                                Visa, Mastercard, Elo
                              </div>
                            </div>
                          </div>
                        </div>

                        <div 
                          className={`p-4 border rounded-lg cursor-pointer transition-colors ${
                            dadosPagamento.metodo === 'pix' ? 'border-primary bg-primary/5' : 'border-border hover:border-primary/50'
                          }`}
                          onClick={() => setDadosPagamento(prev => ({ ...prev, metodo: 'pix' }))}
                        >
                          <div className="flex items-center gap-3">
                            <Smartphone className="h-5 w-5" />
                            <div>
                              <div className="font-medium">PIX</div>
                              <div className="text-sm text-muted-foreground">
                                Aprovação instantânea
                              </div>
                            </div>
                          </div>
                        </div>

                        <div 
                          className={`p-4 border rounded-lg cursor-pointer transition-colors ${
                            dadosPagamento.metodo === 'boleto' ? 'border-primary bg-primary/5' : 'border-border hover:border-primary/50'
                          }`}
                          onClick={() => setDadosPagamento(prev => ({ ...prev, metodo: 'boleto' }))}
                        >
                          <div className="flex items-center gap-3">
                            <CreditCard className="h-5 w-5" />
                            <div>
                              <div className="font-medium">Boleto Bancário</div>
                              <div className="text-sm text-muted-foreground">
                                Aprovação em até 3 dias úteis
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>

                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <Shield className="h-4 w-4" />
                        <span>Pagamento 100% seguro e criptografado</span>
                      </div>
                    </div>
                  )}

                  {etapa === 'dados' && (
                    <div className="space-y-6">
                      {dadosPagamento.metodo === 'cartao' && (
                        <div className="space-y-4">
                          <div>
                            <Label htmlFor="numeroCartao">Número do Cartão</Label>
                            <Input
                              id="numeroCartao"
                              placeholder="0000 0000 0000 0000"
                              value={dadosPagamento.numeroCartao || ''}
                              onChange={(e) => setDadosPagamento(prev => ({ 
                                ...prev, 
                                numeroCartao: formatarValorCartao(e.target.value) 
                              }))}
                              maxLength={19}
                            />
                          </div>

                          <div>
                            <Label htmlFor="nomeCartao">Nome no Cartão</Label>
                            <Input
                              id="nomeCartao"
                              placeholder="Nome como está no cartão"
                              value={dadosPagamento.nomeCartao || ''}
                              onChange={(e) => setDadosPagamento(prev => ({ 
                                ...prev, 
                                nomeCartao: e.target.value 
                              }))}
                            />
                          </div>

                          <div className="grid grid-cols-2 gap-4">
                            <div>
                              <Label htmlFor="validadeCartao">Validade</Label>
                              <Input
                                id="validadeCartao"
                                placeholder="MM/AA"
                                value={dadosPagamento.validadeCartao || ''}
                                onChange={(e) => setDadosPagamento(prev => ({ 
                                  ...prev, 
                                  validadeCartao: formatarValidadeCartao(e.target.value) 
                                }))}
                                maxLength={5}
                              />
                            </div>
                            <div>
                              <Label htmlFor="cvvCartao">CVV</Label>
                              <Input
                                id="cvvCartao"
                                placeholder="000"
                                value={dadosPagamento.cvvCartao || ''}
                                onChange={(e) => setDadosPagamento(prev => ({ 
                                  ...prev, 
                                  cvvCartao: e.target.value.replace(/\D/g, '') 
                                }))}
                                maxLength={4}
                              />
                            </div>
                          </div>
                        </div>
                      )}

                      <div className="space-y-4">
                        <div>
                          <Label htmlFor="cpf">CPF</Label>
                          <Input
                            id="cpf"
                            placeholder="000.000.000-00"
                            value={dadosPagamento.cpf || ''}
                            onChange={(e) => setDadosPagamento(prev => ({ 
                              ...prev, 
                              cpf: formatarCPF(e.target.value) 
                            }))}
                            maxLength={14}
                          />
                        </div>

                        <div>
                          <Label htmlFor="email">E-mail</Label>
                          <Input
                            id="email"
                            type="email"
                            placeholder="seu@email.com"
                            value={dadosPagamento.email || ''}
                            onChange={(e) => setDadosPagamento(prev => ({ 
                              ...prev, 
                              email: e.target.value 
                            }))}
                          />
                        </div>
                      </div>

                      <div className="flex items-center gap-2 text-sm text-muted-foreground">
                        <Lock className="h-4 w-4" />
                        <span>Seus dados estão protegidos com criptografia SSL</span>
                      </div>
                    </div>
                  )}

                  {etapa === 'confirmacao' && (
                    <div className="space-y-6">
                      <div className="p-4 bg-muted/50 rounded-lg">
                        <h3 className="font-medium mb-3">Resumo do Pagamento</h3>
                        <div className="space-y-2 text-sm">
                          <div className="flex justify-between">
                            <span>Plano:</span>
                            <span className="font-medium">{plano.nome}</span>
                          </div>
                          <div className="flex justify-between">
                            <span>Método:</span>
                            <span className="font-medium">
                              {dadosPagamento.metodo === 'cartao' && 'Cartão de Crédito'}
                              {dadosPagamento.metodo === 'pix' && 'PIX'}
                              {dadosPagamento.metodo === 'boleto' && 'Boleto Bancário'}
                            </span>
                          </div>
                          <div className="flex justify-between">
                            <span>Valor:</span>
                            <span className="font-medium">
                              R$ {plano.valorMensal.toFixed(2)}
                              {plano.tipo === 'Trimestral' ? '/trimestre' : '/mês'}
                            </span>
                          </div>
                          {dadosPagamento.metodo === 'cartao' && dadosPagamento.numeroCartao && (
                            <div className="flex justify-between">
                              <span>Cartão:</span>
                              <span className="font-medium">
                                **** **** **** {dadosPagamento.numeroCartao.slice(-4)}
                              </span>
                            </div>
                          )}
                        </div>
                      </div>

                      <div className="p-4 border border-success/20 bg-success/5 rounded-lg">
                        <div className="flex items-center gap-2 text-success">
                          <Check className="h-4 w-4" />
                          <span className="text-sm font-medium">
                            {plano.tipo === 'Gratuito' 
                              ? 'Plano gratuito ativado imediatamente'
                              : 'Pagamento processado com sucesso'
                            }
                          </span>
                        </div>
                      </div>
                    </div>
                  )}

                  <div className="flex gap-4 mt-8">
                    {etapa !== 'metodo' && (
                      <Button variant="outline" onClick={handleVoltar}>
                        Voltar
                      </Button>
                    )}
                    
                    {etapa === 'metodo' && (
                      <Button onClick={handleProximo} className="flex-1">
                        Continuar
                      </Button>
                    )}
                    
                    {etapa === 'dados' && (
                      <Button 
                        onClick={handleProximo} 
                        disabled={!validarDados()}
                        className="flex-1"
                      >
                        Continuar
                      </Button>
                    )}
                    
                    {etapa === 'confirmacao' && (
                      <Button 
                        onClick={handleFinalizarPagamento} 
                        disabled={loading}
                        className="flex-1"
                      >
                        {loading ? (
                          <>
                            <Loader2 className="h-4 w-4 animate-spin mr-2" />
                            Processando...
                          </>
                        ) : (
                          'Finalizar Pagamento'
                        )}
                      </Button>
                    )}
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
