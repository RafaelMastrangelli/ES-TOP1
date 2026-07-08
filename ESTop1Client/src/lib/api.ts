import { 
  Jogador, 
  JogadoresPaginados, 
  FiltrosJogadores, 
  Time, 
  TimesPaginados,
  FiltrosTimes,
  CriarJogadorRequest, 
  CriarTimeRequest, 
  CriarInscricaoRequest,
  ApiResponse,
  FaceitPlayer,
  FaceitStats,
  FaceitMatch,
  FaceitSearchResult,
  ApiErrorResponse,
  ApiRequestError,
  AuthResponse,
  AuthMeResponse,
  AuthAssinatura,
  OpenAIBuscaJogadoresResponse,
  OpenAISugerirFiltrosResponse,
  CheckoutPagamentoResult,
  PagamentoStatusResult,
  PlanoAssinatura
} from '@/types';

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5280/api';

const AUTH_TOKEN_KEY = 'auth_token';
const AUTH_REFRESH_TOKEN_KEY = 'auth_refresh_token';

let refreshInFlight: Promise<boolean> | null = null;

const persistAuthTokens = (data: { token?: string; refreshToken?: string }) => {
  if (data.token) localStorage.setItem(AUTH_TOKEN_KEY, data.token);
  if (data.refreshToken) localStorage.setItem(AUTH_REFRESH_TOKEN_KEY, data.refreshToken);
};

const clearAuthTokens = () => {
  localStorage.removeItem(AUTH_TOKEN_KEY);
  localStorage.removeItem(AUTH_REFRESH_TOKEN_KEY);
};

const refreshAccessToken = async (): Promise<boolean> => {
  if (refreshInFlight) return refreshInFlight;

  const refreshToken = localStorage.getItem(AUTH_REFRESH_TOKEN_KEY);
  if (!refreshToken) return false;

  refreshInFlight = (async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/auth/refresh`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken }),
      });

      if (!response.ok) {
        clearAuthTokens();
        return false;
      }

      const data = keysToCamelCase<AuthResponse>(await response.json());
      persistAuthTokens(data);
      return Boolean(data.token);
    } catch {
      clearAuthTokens();
      return false;
    } finally {
      refreshInFlight = null;
    }
  })();

  return refreshInFlight;
};

const toCamelCase = (key: string): string =>
  key.charAt(0).toLowerCase() + key.slice(1);

const keysToCamelCase = <T>(input: unknown): T => {
  if (Array.isArray(input)) {
    return input.map((item) => keysToCamelCase(item)) as T;
  }

  if (input !== null && typeof input === 'object') {
    return Object.fromEntries(
      Object.entries(input as Record<string, unknown>).map(([key, value]) => [
        toCamelCase(key),
        keysToCamelCase(value),
      ])
    ) as T;
  }

  return input as T;
};

const parseErrorData = async (response: Response): Promise<ApiErrorResponse> => {
  try {
    const responseText = await response.text();
    try {
      return JSON.parse(responseText) as ApiErrorResponse;
    } catch {
      return { message: responseText };
    }
  } catch {
    return { message: `Erro ${response.status}` };
  }
};

const throwApiError = async (response: Response): Promise<never> => {
  const errorData = await parseErrorData(response);
  throw new ApiRequestError(errorData.message || `Erro ${response.status}`, {
    data: errorData,
    status: response.status,
  });
};

// Helper para fazer requisições HTTP
const request = async <T>(
  url: string,
  options: RequestInit = {},
  retryOnUnauthorized = true
): Promise<T> => {
  const token = localStorage.getItem(AUTH_TOKEN_KEY);

  const response = await fetch(url, {
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
    ...options,
  });

  if (response.status === 401 && retryOnUnauthorized && !url.endsWith('/auth/refresh')) {
    const refreshed = await refreshAccessToken();
    if (refreshed) {
      return request<T>(url, options, false);
    }
  }

  if (!response.ok) {
    await throwApiError(response);
  }

  const data = await response.json();
  return keysToCamelCase<T>(data);
};

const fetchWithAuth = async (url: string, options: RequestInit = {}, retryOnUnauthorized = true) => {
  const token = localStorage.getItem(AUTH_TOKEN_KEY);
  const response = await fetch(url, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
  });

  if (response.status === 401 && retryOnUnauthorized && !url.endsWith('/auth/refresh')) {
    const refreshed = await refreshAccessToken();
    if (refreshed) {
      return fetchWithAuth(url, options, false);
    }
  }

  return response;
};

export const api = {
  // Métodos HTTP básicos para usar com axios
  get: async <T = unknown>(url: string) => {
    const response = await fetchWithAuth(`${API_BASE_URL}${url}`);
    
    if (!response.ok) {
      await throwApiError(response);
    }
    
    return { data: keysToCamelCase<T>(await response.json()) };
  },

  post: async <T = unknown>(url: string, data?: unknown) => {
    const response = await fetchWithAuth(`${API_BASE_URL}${url}`, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
    
    if (!response.ok) {
      await throwApiError(response);
    }
    
    return { data: keysToCamelCase<T>(await response.json()) };
  },

  auth: {
    login: async (email: string, senha: string) => {
      return request<AuthResponse>(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        body: JSON.stringify({ email, senha }),
      });
    },

    registro: async (nome: string, email: string, senha: string, tipo: string) => {
      return request<AuthResponse>(`${API_BASE_URL}/auth/registro`, {
        method: 'POST',
        body: JSON.stringify({ nome, email, senha, tipo }),
      });
    },

    me: async () => {
      return request<AuthMeResponse>(`${API_BASE_URL}/auth/me`);
    },

    logout: async () => {
      const refreshToken = localStorage.getItem(AUTH_REFRESH_TOKEN_KEY);
      if (refreshToken) {
        try {
          await request(`${API_BASE_URL}/auth/logout`, {
            method: 'POST',
            body: JSON.stringify({ refreshToken }),
          }, false);
        } catch {
          // Ignora falha de logout remoto e limpa sessão local
        }
      }
      clearAuthTokens();
    },
  },

  pagamentos: {
    checkout: async (plano: string, metodo: string = 'checkout') => {
      return request<CheckoutPagamentoResult>(`${API_BASE_URL}/pagamentos/checkout`, {
        method: 'POST',
        body: JSON.stringify({ plano, metodo }),
      });
    },

    obterStatus: async (pagamentoId: string) => {
      return request<PagamentoStatusResult>(`${API_BASE_URL}/pagamentos/${pagamentoId}/status`);
    },

    historico: async () => {
      return request<PagamentoStatusResult[]>(`${API_BASE_URL}/pagamentos/historico`);
    },

    simularAprovacao: async (pagamentoId: string) => {
      return request<CheckoutPagamentoResult>(`${API_BASE_URL}/pagamentos/${pagamentoId}/simular-aprovacao`, {
        method: 'POST',
      }, false);
    },
  },

  assinaturas: {
    obterPlanos: async () => {
      return request<AuthAssinatura[]>(`${API_BASE_URL}/assinaturas/planos`);
    },

    obterMinha: async () => {
      return request<AuthAssinatura>(`${API_BASE_URL}/assinaturas/minha`);
    },

    criar: async (plano: string) => {
      return request<AuthAssinatura>(`${API_BASE_URL}/assinaturas/criar`, {
        method: 'POST',
        body: JSON.stringify({ plano }),
      });
    },

    cancelar: async (id: string) => {
      return request<AuthAssinatura>(`${API_BASE_URL}/assinaturas/${id}/cancelar`, {
        method: 'POST',
      });
    },

    renovar: async (id: string) => {
      return request<AuthAssinatura>(`${API_BASE_URL}/assinaturas/${id}/renovar`, {
        method: 'POST',
      });
    },
  },

  jogadores: {
    listar: async (filtros?: FiltrosJogadores): Promise<JogadoresPaginados> => {
      const params = new URLSearchParams();
      
      if (filtros?.q) params.append('q', filtros.q);
      if (filtros?.pais) params.append('pais', filtros.pais);
      if (filtros?.status) params.append('status', filtros.status);
      if (filtros?.disp) params.append('disp', filtros.disp);
      if (filtros?.funcao) params.append('funcao', filtros.funcao);
      if (filtros?.maxIdade) params.append('maxIdade', filtros.maxIdade.toString());
      if (filtros?.ordenar) params.append('ordenar', filtros.ordenar);
      if (filtros?.page) params.append('page', filtros.page.toString());
      if (filtros?.pageSize) params.append('pageSize', filtros.pageSize.toString());

      return request<JogadoresPaginados>(`${API_BASE_URL}/jogadores?${params}`);
    },

    buscarPorId: async (id: string): Promise<Jogador> => {
      return request<Jogador>(`${API_BASE_URL}/jogadores/${id}`);
    },

    criar: async (dados: CriarJogadorRequest): Promise<Jogador> => {
      return request<Jogador>(`${API_BASE_URL}/jogadores`, {
        method: 'POST',
        body: JSON.stringify(dados),
      });
    },

    alterarVisibilidade: async (id: string, visivel: boolean): Promise<ApiResponse<null>> => {
      return request<ApiResponse<null>>(`${API_BASE_URL}/jogadores/${id}/visibilidade?on=${visivel}`, {
        method: 'PUT',
      });
    },

    meuPerfil: async (): Promise<Jogador> => {
      return request<Jogador>(`${API_BASE_URL}/jogadores/meu-perfil`);
    },

    atualizarPerfil: async (dados: Partial<Jogador>): Promise<Jogador> => {
      return request<Jogador>(`${API_BASE_URL}/jogadores/meu-perfil`, {
        method: 'PUT',
        body: JSON.stringify(dados),
      });
    },

    criarPerfil: async (): Promise<Jogador> => {
      return request<Jogador>(`${API_BASE_URL}/jogadores/meu-perfil`, {
        method: 'POST',
      });
    },
  },

  times: {
    listar: async (filtros?: FiltrosTimes): Promise<TimesPaginados> => {
      const params = new URLSearchParams();
      
      if (filtros?.nome) params.append('nome', filtros.nome);
      if (filtros?.tier) params.append('tier', filtros.tier);
      if (filtros?.contratando) params.append('contratando', filtros.contratando);
      if (filtros?.ordenar) params.append('ordenar', filtros.ordenar);
      if (filtros?.page) params.append('page', filtros.page.toString());
      if (filtros?.pageSize) params.append('pageSize', filtros.pageSize.toString());

      return request<TimesPaginados>(`${API_BASE_URL}/times?${params}`);
    },

    buscarPorId: async (id: string): Promise<Time> => {
      return request<Time>(`${API_BASE_URL}/times/${id}`);
    },

    criar: async (dados: CriarTimeRequest): Promise<Time> => {
      return request<Time>(`${API_BASE_URL}/times`, {
        method: 'POST',
        body: JSON.stringify(dados),
      });
    },

    meuTime: async (): Promise<Time> => {
      return request<Time>(`${API_BASE_URL}/times/meu-time`);
    },

    atualizarTime: async (dados: Partial<Time>): Promise<Time> => {
      return request<Time>(`${API_BASE_URL}/times/meu-time`, {
        method: 'PUT',
        body: JSON.stringify(dados),
      });
    },
  },

  inscricoes: {
    criar: async (dados: CriarInscricaoRequest): Promise<ApiResponse<{ inscricaoId: string }>> => {
      return request<ApiResponse<{ inscricaoId: string }>>(`${API_BASE_URL}/inscricoes`, {
        method: 'POST',
        body: JSON.stringify(dados),
      });
    },

    pagar: async (id: string): Promise<ApiResponse<null>> => {
      return request<ApiResponse<null>>(`${API_BASE_URL}/inscricoes/${id}/pagar`, {
        method: 'POST',
      });
    },

    aprovar: async (id: string): Promise<ApiResponse<null>> => {
      return request<ApiResponse<null>>(`${API_BASE_URL}/inscricoes/${id}/aprovar`, {
        method: 'POST',
      });
    },
  },

  faceit: {
    buscarJogador: async (nickname: string): Promise<FaceitPlayer> => {
      return request<FaceitPlayer>(`${API_BASE_URL}/integracoes/faceit/jogador/${encodeURIComponent(nickname)}`);
    },

    buscarEstatisticas: async (playerId: string): Promise<FaceitStats> => {
      return request<FaceitStats>(`${API_BASE_URL}/integracoes/faceit/estatisticas/${playerId}`);
    },

    buscarPartidas: async (playerId: string, limite: number = 5): Promise<FaceitMatch[]> => {
      return request<FaceitMatch[]>(`${API_BASE_URL}/integracoes/faceit/partidas/${playerId}?limite=${limite}`);
    },

    buscarDadosCompletos: async (nickname: string): Promise<FaceitSearchResult> => {
      try {
        // Buscar jogador pelo nickname
        const player = await request<FaceitPlayer>(`${API_BASE_URL}/integracoes/faceit/jogador/${encodeURIComponent(nickname)}`);
        
        if (!player) {
          return { player: null, stats: null, matches: [] };
        }

        // Buscar estatísticas e partidas em paralelo
        const [stats, matches] = await Promise.allSettled([
          request<FaceitStats>(`${API_BASE_URL}/integracoes/faceit/estatisticas/${player.playerId}`),
          request<FaceitMatch[]>(`${API_BASE_URL}/integracoes/faceit/partidas/${player.playerId}?limite=5`)
        ]);

        return {
          player,
          stats: stats.status === 'fulfilled' ? stats.value : null,
          matches: matches.status === 'fulfilled' ? matches.value : []
        };
      } catch (error) {
        console.error('Erro ao buscar dados completos FACEIT:', error);
        return { player: null, stats: null, matches: [] };
      }
    },
  },

  openai: {
    buscarJogadores: async (consulta: string): Promise<OpenAIBuscaJogadoresResponse> => {
      return request<OpenAIBuscaJogadoresResponse>(
        `${API_BASE_URL}/integracoes/openai/buscar-jogadores?consulta=${encodeURIComponent(consulta)}`
      );
    },

    sugerirFiltros: async (descricao: string): Promise<OpenAISugerirFiltrosResponse> => {
      return request<OpenAISugerirFiltrosResponse>(
        `${API_BASE_URL}/integracoes/openai/sugerir-filtros?descricao=${encodeURIComponent(descricao)}`
      );
    },
  },
};
