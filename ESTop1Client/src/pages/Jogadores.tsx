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
  const [aiData, setAiData] = useState<any>(null);
  const [isLoadingAI, setIsLoadingAI] = useState(false);
  const [activeSearchType, setActiveSearchType] = useState<'local' | 'faceit' | 'ia' | null>(null);

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
    // Limpar resultados anteriores
    setAiData(null);
    setIsLoadingAI(false);
    setActiveSearchType('faceit');
    
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
    setActiveSearchType(null);
  };

  const handleAISearch = async (consulta: string) => {
    // Limpar resultados anteriores
    setFaceitData(null);
    setIsLoadingFaceit(false);
    setActiveSearchType('ia');
    
    setIsLoadingAI(true);
    try {
      // Usar endpoint de teste para desenvolvimento (sem verificação de assinatura)
      const result = await api.openai.buscarJogadoresTeste(consulta);
      setAiData(result);
    } catch (error: any) {
      console.error('Erro ao buscar com IA:', error);
      
      // Handle specific error cases
      if (error.response?.status === 403) {
        alert('Acesso negado: Você precisa de uma assinatura ativa com acesso à busca por IA para usar esta funcionalidade.');
      } else if (error.response?.status === 401) {
        alert('Não autorizado: Faça login novamente para continuar.');
      } else {
        alert(`Erro ao buscar jogadores com IA: ${error.message || 'Erro desconhecido'}`);
      }
      
      setAiData({ jogadores: [], total: 0, consultaOriginal: consulta, consultaIA: '' });
    } finally {
      setIsLoadingAI(false);
    }
  };

  const handleCloseAI = () => {
    setAiData(null);
    setActiveSearchType(null);
  };

  const handleClearAllResults = () => {
    setFaceitData(null);
    setAiData(null);
    setIsLoadingFaceit(false);
    setIsLoadingAI(false);
    setActiveSearchType(null);
  };

  return (
    <div className="min-h-screen bg-background">
      <Navbar />

      <main className="container mx-auto px-4 py-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold mb-2">Jogadores Profissionais</h1>
          <div className="flex items-center gap-4">
            <p className="text-muted-foreground">
              {activeSearchType === 'faceit' && 'Resultados da busca FACEIT'}
              {activeSearchType === 'ia' && 'Resultados da busca com IA'}
              {activeSearchType === 'local' && `Explore ${data?.total || 0} jogadores cadastrados`}
              {activeSearchType === null && `Explore ${data?.total || 0} jogadores cadastrados`}
            </p>
            {activeSearchType && (
              <div className="flex items-center gap-2">
                <div className={`w-2 h-2 rounded-full ${
                  activeSearchType === 'faceit' ? 'bg-blue-500' :
                  activeSearchType === 'ia' ? 'bg-purple-500' :
                  'bg-green-500'
                }`} />
                <span className="text-sm text-muted-foreground">
                  {activeSearchType === 'faceit' ? 'FACEIT' :
                   activeSearchType === 'ia' ? 'IA' :
                   'Local'}
                </span>
              </div>
            )}
          </div>
        </div>

        <div className="space-y-6">
          <FiltrosJogadores 
            filtros={filtros} 
            onChange={(newFiltros) => {
              // Limpar resultados de outras buscas quando fazer busca local
              handleClearAllResults();
              setActiveSearchType('local');
              setFiltros(newFiltros);
            }}
            onFaceitSearch={handleFaceitSearch}
            onAISearch={handleAISearch}
          />

          {/* Loading FACEIT */}
          {isLoadingFaceit && (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-6 w-6 animate-spin text-primary mr-2" />
              <span>Buscando dados na FACEIT...</span>
            </div>
          )}

          {/* Loading IA */}
          {isLoadingAI && (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-6 w-6 animate-spin text-purple-500 mr-2" />
              <span>Processando consulta com IA...</span>
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

          {/* Dados IA */}
          {aiData && (
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <div>
                  <h3 className="text-lg font-semibold text-purple-600">Resultado da Busca com IA</h3>
                  <p className="text-sm text-muted-foreground">
                    Consulta: "{aiData.consultaOriginal}" • {aiData.total} jogador(es) encontrado(s)
                  </p>
                </div>
                <Button variant="outline" size="sm" onClick={handleCloseAI}>
                  <X className="h-4 w-4 mr-2" />
                  Fechar
                </Button>
              </div>
              
              {aiData.jogadores.length > 0 ? (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                  {aiData.jogadores.map((jogador: any) => (
                    <JogadorCard key={jogador.id} jogador={jogador} />
                  ))}
                </div>
              ) : (
                <div className="text-center py-8">
                  <p className="text-muted-foreground">Nenhum jogador encontrado com a consulta da IA.</p>
                </div>
              )}
            </div>
          )}

          {/* Resultados da Busca Local - só exibe quando busca local está ativa ou nenhuma busca específica está ativa */}
          {(activeSearchType === 'local' || activeSearchType === null) && (
            <>
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
            </>
          )}

          {/* Mensagem quando nenhuma busca está ativa e não há resultados */}
          {activeSearchType === null && !data && !isLoading && !error && (
            <div className="text-center py-16">
              <p className="text-muted-foreground">Use os filtros acima para buscar jogadores.</p>
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default Jogadores;
