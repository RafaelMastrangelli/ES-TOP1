import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { api } from '@/lib/api';
import { FiltrosJogadores as Filtros, FaceitSearchResult } from '@/types';
import Navbar from '@/components/Navbar';
import JogadorCard from '@/components/JogadorCard';
import FiltrosJogadores from '@/components/FiltrosJogadores';
import FaceitPlayerCard from '@/components/FaceitPlayerCard';
import { Button } from '@/components/ui/button';
import { ChevronLeft, ChevronRight, Loader2, X } from 'lucide-react';

const Jogadores = () => {
  const [filtros, setFiltros] = useState<Filtros>({
    page: 1,
    pageSize: 12,
  });

  const [faceitData, setFaceitData] = useState<FaceitSearchResult | null>(null);
  const [isLoadingFaceit, setIsLoadingFaceit] = useState(false);

  const { data, isLoading, error } = useQuery({
    queryKey: ['jogadores', filtros],
    queryFn: () => api.jogadores.listar(filtros),
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

  const handleFaceitSearch = async (nickname: string) => {
    setIsLoadingFaceit(true);
    try {
      const result = await api.faceit.buscarDadosCompletos(nickname);
      setFaceitData(result);
    } catch (error) {
      console.error('Erro ao buscar dados FACEIT:', error);
      setFaceitData({ player: null, stats: null, matches: [] });
    } finally {
      setIsLoadingFaceit(false);
    }
  };

  const handleCloseFaceit = () => {
    setFaceitData(null);
  };

  return (
    <div className="min-h-screen bg-background">
      <Navbar />

      <main className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold mb-2">Jogadores Profissionais</h1>
          <p className="text-muted-foreground">
            Explore {data?.total || 0} jogadores cadastrados
          </p>
        </div>

        <div className="space-y-6">
          <FiltrosJogadores 
            filtros={filtros} 
            onChange={setFiltros} 
            onFaceitSearch={handleFaceitSearch}
          />

          {/* Loading FACEIT */}
          {isLoadingFaceit && (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-6 w-6 animate-spin text-primary mr-2" />
              <span>Buscando dados na FACEIT...</span>
            </div>
          )}

          {/* Dados FACEIT */}
          {faceitData && (
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <h3 className="text-lg font-semibold">Resultado da Busca FACEIT</h3>
                <Button variant="outline" size="sm" onClick={handleCloseFaceit}>
                  <X className="h-4 w-4 mr-2" />
                  Fechar
                </Button>
              </div>
              <FaceitPlayerCard data={faceitData} onClose={handleCloseFaceit} />
            </div>
          )}

          {/* Loading Local */}
          {isLoading && (
            <div className="flex items-center justify-center py-16">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
          )}

          {error && (
            <div className="text-center py-16">
              <p className="text-destructive">Erro ao carregar jogadores. Tente novamente.</p>
            </div>
          )}

          {data && data.items.length === 0 && (
            <div className="text-center py-16">
              <p className="text-muted-foreground">Nenhum jogador encontrado com os filtros selecionados.</p>
            </div>
          )}

          {data && data.items.length > 0 && (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                {data.items.map((jogador) => (
                  <JogadorCard key={jogador.id} jogador={jogador} />
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

export default Jogadores;
