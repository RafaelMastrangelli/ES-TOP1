import { useParams, Link } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import Navbar from '@/components/Navbar';
import JogadorCard from '@/components/JogadorCard';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { ArrowLeft, Users, Trophy, Loader2 } from 'lucide-react';
import { obterBandeiraPais } from '@/lib/utils';

const TimeDetalhes = () => {
  const { id } = useParams<{ id: string }>();

  const { data: time, isLoading, error } = useQuery({
    queryKey: ['time', id],
    queryFn: () => api.times.buscarPorId(id!),
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

  if (error || !time) {
    return (
      <div className="min-h-screen bg-background">
        <Navbar />
        <div className="container mx-auto px-4 py-8 text-center">
          <p className="text-destructive">Time não encontrado.</p>
          <Button asChild className="mt-4">
            <Link to="/times">Voltar para Times</Link>
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      <Navbar />

      <main className="container mx-auto px-4 py-8">
        <Button variant="ghost" asChild className="mb-6">
          <Link to="/times">
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Link>
        </Button>

        <Card className="p-8 mb-8">
          <div className="flex items-start justify-between">
            <div className="space-y-4">
              <div>
                <h1 className="text-4xl font-bold mb-2">{time.nome}</h1>
                <p className="text-xl text-muted-foreground">
                  {obterBandeiraPais(time.pais)} {time.pais}
                </p>
              </div>

              <div className="flex gap-4">
                <div className="flex items-center gap-2">
                  <Users className="h-5 w-5 text-primary" />
                  <span className="font-mono font-medium">
                    {time.jogadores?.length || 0} jogadores
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  <Trophy className="h-5 w-5 text-accent" />
                  <span className="font-mono font-medium">Time Profissional</span>
                </div>
              </div>
            </div>

            <Button>Contatar Time</Button>
          </div>
        </Card>

        {time.jogadores && time.jogadores.length > 0 ? (
          <div>
            <h2 className="text-2xl font-bold mb-6">Roster</h2>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
              {time.jogadores.map((jogador) => (
                <JogadorCard key={jogador.id} jogador={jogador} />
              ))}
            </div>
          </div>
        ) : (
          <Card className="p-12 text-center">
            <p className="text-muted-foreground">Nenhum jogador cadastrado neste time.</p>
          </Card>
        )}
      </main>
    </div>
  );
};

export default TimeDetalhes;
