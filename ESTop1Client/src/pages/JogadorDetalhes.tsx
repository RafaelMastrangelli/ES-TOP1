import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import Navbar from '@/components/Navbar';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { ArrowLeft, TrendingUp, Users, Target, Loader2 } from 'lucide-react';
import { formatarMoeda, obterBandeiraPais } from '@/lib/utils';
import playerPlaceholder from '@/assets/player-placeholder.jpg';

const JogadorDetalhes = () => {
  const { id } = useParams<{ id: string }>();

  const { data: jogador, isLoading, error } = useQuery({
    queryKey: ['jogador', id],
    queryFn: () => api.jogadores.buscarPorId(id!),
    enabled: !!id,
  });

  if (isLoading) {
    return (
      <div className="min-h-screen bg-background">
        <Navbar />
        <div className="flex items-center justify-center py-16">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </div>
      </div>
    );
  }

  if (error || !jogador) {
    return (
      <div className="min-h-screen bg-background">
        <Navbar />
        <div className="container mx-auto px-4 py-8 text-center">
          <p className="text-destructive">Jogador não encontrado.</p>
          <Button asChild className="mt-4">
            <Link to="/jogadores">Voltar para Jogadores</Link>
          </Button>
        </div>
      </div>
    );
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Profissional':
        return 'bg-success text-white';
      case 'Aposentado':
        return 'bg-muted text-muted-foreground';
      case 'Amador':
        return 'bg-warning text-white';
      default:
        return 'bg-muted';
    }
  };

  const estatisticaGeral = jogador.estatisticas?.find(e => e.periodo === 'Geral');

  return (
    <div className="min-h-screen bg-background">
      <Navbar />

      <main className="container mx-auto px-4 py-8">
        <Button variant="ghost" asChild className="mb-6">
          <Link to="/jogadores">
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Link>
        </Button>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Perfil */}
          <div className="lg:col-span-1">
            <Card className="overflow-hidden">
              <div className="aspect-square relative bg-gradient-dark">
                <img 
                  src={jogador.fotoUrl || playerPlaceholder} 
                  alt={jogador.apelido}
                  className="w-full h-full object-cover"
                />
              </div>
              
              <div className="p-6 space-y-4">
                <div>
                  <h1 className="text-3xl font-bold mb-2">{jogador.apelido}</h1>
                  <p className="text-lg text-muted-foreground">
                    {obterBandeiraPais(jogador.pais)} {jogador.pais} • {jogador.idade} anos
                  </p>
                </div>

                <div className="flex gap-2">
                  <Badge className={getStatusColor(jogador.status)}>
                    {jogador.status}
                  </Badge>
                  <Badge variant="secondary">{jogador.funcaoPrincipal}</Badge>
                </div>

                <div className="pt-4 border-t space-y-3">
                  <div className="flex justify-between">
                    <span className="text-sm text-muted-foreground">Time Atual</span>
                    <span className="text-sm font-medium">
                      {jogador.timeAtual ? (
                        <Link to={`/times/${jogador.timeAtualId}`} className="hover:text-primary">
                          {jogador.timeAtual.nome}
                        </Link>
                      ) : (
                        'Sem time'
                      )}
                    </span>
                  </div>

                  <div className="flex justify-between">
                    <span className="text-sm text-muted-foreground">Disponibilidade</span>
                    <span className="text-sm font-medium">
                      {jogador.disponibilidade === 'EmTime' ? 'Em Time' : 
                       jogador.disponibilidade === 'Livre' ? 'Livre' : 'Em Teste'}
                    </span>
                  </div>

                  <div className="flex justify-between">
                    <span className="text-sm text-muted-foreground">Valor de Mercado</span>
                    <span className="text-sm font-bold text-success">
                      {formatarMoeda(jogador.valorDeMercado)}
                    </span>
                  </div>
                </div>

                <Button className="w-full mt-4">Contatar Jogador</Button>
              </div>
            </Card>
          </div>

          {/* Estatísticas */}
          <div className="lg:col-span-2 space-y-6">
            <Card className="p-6">
              <h2 className="text-2xl font-bold mb-6 flex items-center gap-2">
                <TrendingUp className="h-6 w-6 text-primary" />
                Estatísticas
              </h2>

              {estatisticaGeral ? (
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <div className="text-center p-6 rounded-lg bg-primary/10">
                    <div className="text-4xl font-bold font-mono text-primary mb-2">
                      {estatisticaGeral.rating.toFixed(2)}
                    </div>
                    <div className="text-sm text-muted-foreground">Rating Geral</div>
                  </div>

                  <div className="text-center p-6 rounded-lg bg-secondary/10">
                    <div className="text-4xl font-bold font-mono text-secondary mb-2">
                      {estatisticaGeral.kd.toFixed(2)}
                    </div>
                    <div className="text-sm text-muted-foreground">K/D Ratio</div>
                  </div>

                  <div className="text-center p-6 rounded-lg bg-accent/10">
                    <div className="text-4xl font-bold font-mono text-accent mb-2">
                      {estatisticaGeral.partidasJogadas}
                    </div>
                    <div className="text-sm text-muted-foreground">Partidas Jogadas</div>
                  </div>
                </div>
              ) : (
                <p className="text-muted-foreground text-center py-8">
                  Estatísticas não disponíveis
                </p>
              )}
            </Card>

            {jogador.estatisticas && jogador.estatisticas.length > 1 && (
              <Card className="p-6">
                <h2 className="text-2xl font-bold mb-6 flex items-center gap-2">
                  <Target className="h-6 w-6 text-primary" />
                  Estatísticas por Período
                </h2>

                <div className="space-y-4">
                  {jogador.estatisticas
                    .filter(e => e.periodo !== 'Geral')
                    .map((stat) => (
                      <div key={stat.id} className="flex items-center justify-between p-4 rounded-lg border">
                        <div>
                          <div className="font-semibold">{stat.periodo}</div>
                          <div className="text-sm text-muted-foreground">
                            {stat.partidasJogadas} partidas
                          </div>
                        </div>
                        <div className="flex gap-6 font-mono">
                          <div className="text-center">
                            <div className="text-sm text-muted-foreground">Rating</div>
                            <div className="font-bold text-primary">{stat.rating.toFixed(2)}</div>
                          </div>
                          <div className="text-center">
                            <div className="text-sm text-muted-foreground">K/D</div>
                            <div className="font-bold text-secondary">{stat.kd.toFixed(2)}</div>
                          </div>
                        </div>
                      </div>
                    ))}
                </div>
              </Card>
            )}

            <Card className="p-6">
              <h2 className="text-2xl font-bold mb-4 flex items-center gap-2">
                <Users className="h-6 w-6 text-primary" />
                Informações Adicionais
              </h2>
              <div className="space-y-3">
                <div className="flex justify-between py-2 border-b">
                  <span className="text-muted-foreground">Função Principal</span>
                  <span className="font-medium">{jogador.funcaoPrincipal}</span>
                </div>
                <div className="flex justify-between py-2 border-b">
                  <span className="text-muted-foreground">País</span>
                  <span className="font-medium">{obterBandeiraPais(jogador.pais)} {jogador.pais}</span>
                </div>
                <div className="flex justify-between py-2 border-b">
                  <span className="text-muted-foreground">Idade</span>
                  <span className="font-medium">{jogador.idade} anos</span>
                </div>
                <div className="flex justify-between py-2">
                  <span className="text-muted-foreground">Status</span>
                  <Badge className={getStatusColor(jogador.status)}>
                    {jogador.status}
                  </Badge>
                </div>
              </div>
            </Card>
          </div>
        </div>
      </main>
    </div>
  );
};

export default JogadorDetalhes;
