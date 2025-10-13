import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import Navbar from '@/components/Navbar';
import TimeCard from '@/components/TimeCard';
import { Loader2 } from 'lucide-react';

const Times = () => {
  const { data: times, isLoading, error } = useQuery({
    queryKey: ['times'],
    queryFn: api.times.listar,
  });

  return (
    <div className="min-h-screen bg-background">
      <Navbar />

      <main className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold mb-2">Times Profissionais</h1>
          <p className="text-muted-foreground">
            Explore {times?.length || 0} times cadastrados
          </p>
        </div>

        {isLoading && (
          <div className="flex items-center justify-center py-16">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
          </div>
        )}

        {error && (
          <div className="text-center py-16">
            <p className="text-destructive">Erro ao carregar times. Tente novamente.</p>
          </div>
        )}

        {times && times.length === 0 && (
          <div className="text-center py-16">
            <p className="text-muted-foreground">Nenhum time encontrado.</p>
          </div>
        )}

        {times && times.length > 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {times.map((time) => (
              <TimeCard key={time.id} time={time} />
            ))}
          </div>
        )}
      </main>
    </div>
  );
};

export default Times;
