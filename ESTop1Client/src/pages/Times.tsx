import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { FiltrosTimes } from '@/types';
import TimeCard from '@/components/TimeCard';
import FiltrosTimesComponent from '@/components/FiltrosTimes';
import { Button } from '@/components/ui/button';
import { ChevronLeft, ChevronRight, Loader2 } from 'lucide-react';

const Times = () => {
  const [filtros, setFiltros] = useState<FiltrosTimes>({
    page: 1,
    pageSize: 12,
  });

  const { data, isLoading, error } = useQuery({
    queryKey: ['times', filtros],
    queryFn: () => api.times.listar(filtros),
  });

  const totalPaginas = data ? Math.ceil(data.total / (filtros.pageSize || 12)) : 0;

  const handlePaginaAnterior = () => {
    if (filtros.page && filtros.page > 1) {
      setFiltros({ ...filtros, page: filtros.page - 1 });
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  };

  const handleProximaPagina = () => {
    if (filtros.page && filtros.page < totalPaginas) {
      setFiltros({ ...filtros, page: filtros.page + 1 });
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  };

  return (
    <div className="min-h-screen bg-background">

      <main className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold mb-2">Times Profissionais</h1>
          <p className="text-muted-foreground">
            Explore {data?.total || 0} times cadastrados
          </p>
        </div>

        <div className="space-y-6">
          <FiltrosTimesComponent 
            filtros={filtros} 
            onChange={setFiltros}
          />

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

          {data && data.items.length === 0 && (
            <div className="text-center py-16">
              <p className="text-muted-foreground">Nenhum time encontrado com os filtros selecionados.</p>
            </div>
          )}

          {data && data.items.length > 0 && (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                {data.items.map((time) => (
                  <TimeCard key={time.id} time={time} />
                ))}
              </div>

              {totalPaginas > 1 && (
                <div className="flex items-center justify-center gap-4 pt-8">
                  <Button
                    variant="outline"
                    onClick={handlePaginaAnterior}
                    disabled={filtros.page === 1}
                  >
                    <ChevronLeft className="h-4 w-4 mr-2" />
                    Anterior
                  </Button>

                  <span className="text-sm text-muted-foreground">
                    Página {filtros.page} de {totalPaginas}
                  </span>

                  <Button
                    variant="outline"
                    onClick={handleProximaPagina}
                    disabled={filtros.page === totalPaginas}
                  >
                    Próxima
                    <ChevronRight className="h-4 w-4 ml-2" />
                  </Button>
                </div>
              )}
            </>
          )}
        </div>
      </main>
    </div>
  );
};

export default Times;
