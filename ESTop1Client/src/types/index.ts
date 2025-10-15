export type FuncaoPrincipal = 'Entry' | 'Suporte' | 'Awp' | 'Igl' | 'Lurker';
export type StatusJogador = 'Profissional' | 'Aposentado' | 'Amador';
export type Disponibilidade = 'EmTime' | 'Livre' | 'Teste';

export interface Estatistica {
  id: string;
  jogadorId: string;
  periodo: string;
  rating: number;
  kd: number;
  partidasJogadas: number;
}

export interface Time {
  id: string;
  nome: string;
  pais: string;
  tier?: number;
  contratando?: boolean;
  logoUrl?: string;
  jogadores?: Jogador[];
}

export interface Jogador {
  id: string;
  apelido: string;
  pais: string;
  idade: number;
  funcaoPrincipal: FuncaoPrincipal;
  status: StatusJogador;
  disponibilidade: Disponibilidade;
  timeAtualId?: string;
  timeAtual?: Time;
  valorDeMercado: number;
  fotoUrl?: string;
  visivel: boolean;
  estatisticas?: Estatistica[];
  ratingGeral?: number;
}

export interface JogadoresPaginados {
  total: number;
  page: number;
  pageSize: number;
  items: Jogador[];
}

export interface TimesPaginados {
  total: number;
  page: number;
  pageSize: number;
  items: Time[];
}

export interface FiltrosJogadores {
  q?: string;
  pais?: string;
  status?: StatusJogador;
  disp?: Disponibilidade;
  funcao?: FuncaoPrincipal;
  maxIdade?: number;
  ordenar?: 'rating_desc' | 'valor_desc' | 'apelido_asc';
  page?: number;
  pageSize?: number;
}

export interface FiltrosTimes {
  nome?: string;
  tier?: '1' | '2' | '3';
  contratando?: 'true' | 'false';
  ordenar?: 'nome_asc' | 'tier_asc' | 'tier_desc';
  page?: number;
  pageSize?: number;
}

// DTOs para criação
export interface CriarJogadorRequest {
  apelido: string;
  pais?: string;
  idade: number;
  funcaoPrincipal: FuncaoPrincipal;
  status: StatusJogador;
  disponibilidade: Disponibilidade;
  timeAtualId?: string;
  valorDeMercado: number;
}

export interface CriarTimeRequest {
  nome: string;
  pais?: string;
}

export interface CriarInscricaoRequest {
  apelido: string;
  pais?: string;
  idade: number;
  funcaoPrincipal: FuncaoPrincipal;
  rating?: number;
  kd?: number;
  partidasJogadas?: number;
}

// Response types
export interface ApiResponse<T> {
  data?: T;
  message?: string;
  error?: string;
}

// FACEIT API Types
export interface FaceitPlayer {
  playerId: string;
  nickname: string;
  country: string;
  avatar: string;
  games?: {
    cs2?: {
      skillLevel: number;
      faceitElo: number;
    };
  };
}

export interface FaceitStats {
  lifetime?: {
    winRate: string;
    matches: string;
    averageKDRatio: string;
    headshots: string;
    averageKills: string;
    averageDeaths: string;
    averageAssists: string;
    averageKRRatio: string;
    averageTripleKills: string;
    averageQuadroKills: string;
    averagePentaKills: string;
  };
}

export interface FaceitMatch {
  matchId: string;
  startedAt: number;
  finishedAt: number;
  teams?: {
    faction1?: {
      teamId: string;
      nickname: string;
      avatar: string;
      type: string;
      players: Array<{
        playerId: string;
        nickname: string;
        avatar: string;
        gamePlayerId: string;
        gamePlayerName: string;
        faceitUrl: string;
      }>;
    };
    faction2?: {
      teamId: string;
      nickname: string;
      avatar: string;
      type: string;
      players: Array<{
        playerId: string;
        nickname: string;
        avatar: string;
        gamePlayerId: string;
        gamePlayerName: string;
        faceitUrl: string;
      }>;
    };
  };
  winner: string;
  results?: {
    score?: {
      faction1: number;
      faction2: number;
    };
  };
}

export interface FaceitSearchResult {
  player: FaceitPlayer | null;
  stats: FaceitStats | null;
  matches: FaceitMatch[];
}
