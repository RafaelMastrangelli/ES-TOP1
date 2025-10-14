import { FaceitSearchResult } from '@/types';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { ExternalLink, Trophy, Target, TrendingUp, Users, Calendar } from 'lucide-react';
import { Button } from './ui/button';
import { obterBandeiraPais } from '@/lib/utils';

interface FaceitPlayerCardProps {
  data: FaceitSearchResult;
  onClose: () => void;
}

const FaceitPlayerCard = ({ data, onClose }: FaceitPlayerCardProps) => {
  const { player, stats, matches } = data;

  if (!player) {
    return (
      <Card className="border-destructive">
        <CardContent className="p-6 text-center">
          <p className="text-destructive">Jogador não encontrado na FACEIT</p>
          <Button variant="outline" onClick={onClose} className="mt-4">
            Fechar
          </Button>
        </CardContent>
      </Card>
    );
  }

  const formatDate = (timestamp: number) => {
    return new Date(timestamp * 1000).toLocaleDateString('pt-BR');
  };

  const getSkillLevelColor = (level: number) => {
    if (level >= 8) return 'bg-red-500';
    if (level >= 6) return 'bg-orange-500';
    if (level >= 4) return 'bg-yellow-500';
    if (level >= 2) return 'bg-green-500';
    return 'bg-gray-500';
  };

  return (
    <Card className="border-primary">
      <CardHeader>
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <img 
              src={player.avatar} 
              alt={player.nickname}
              className="w-16 h-16 rounded-full"
            />
            <div>
              <CardTitle className="text-2xl">{player.nickname}</CardTitle>
              <p className="text-muted-foreground">
                {obterBandeiraPais(player.country)} {player.country}
              </p>
            </div>
          </div>
          <div className="flex gap-2">
            <Button variant="outline" size="sm" onClick={onClose}>
              Fechar
            </Button>
            <Button 
              variant="outline" 
              size="sm"
              onClick={() => window.open(`https://www.faceit.com/en/players/${player.nickname}`, '_blank')}
            >
              <ExternalLink className="h-4 w-4 mr-2" />
              Ver no FACEIT
            </Button>
          </div>
        </div>
      </CardHeader>

      <CardContent className="space-y-6">
        {/* Informações do Jogador */}
        {player.games?.cs2 && (
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <h4 className="font-semibold flex items-center gap-2">
                <Trophy className="h-4 w-4" />
                Nível de Habilidade
              </h4>
              <div className="flex items-center gap-2">
                <Badge className={`${getSkillLevelColor(player.games.cs2.skillLevel)} text-white`}>
                  Nível {player.games.cs2.skillLevel}
                </Badge>
                <span className="text-sm text-muted-foreground">
                  {player.games.cs2.faceitElo} ELO
                </span>
              </div>
            </div>
          </div>
        )}

        {/* Estatísticas */}
        {stats?.lifetime && (
          <div className="space-y-4">
            <h4 className="font-semibold flex items-center gap-2">
              <TrendingUp className="h-4 w-4" />
              Estatísticas Lifetime
            </h4>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div className="text-center p-3 bg-muted rounded-lg">
                <div className="text-2xl font-bold text-primary">
                  {stats.lifetime.winRate}
                </div>
                <div className="text-sm text-muted-foreground">Win Rate</div>
              </div>
              <div className="text-center p-3 bg-muted rounded-lg">
                <div className="text-2xl font-bold text-primary">
                  {stats.lifetime.matches}
                </div>
                <div className="text-sm text-muted-foreground">Partidas</div>
              </div>
              <div className="text-center p-3 bg-muted rounded-lg">
                <div className="text-2xl font-bold text-primary">
                  {stats.lifetime.averageKDRatio}
                </div>
                <div className="text-sm text-muted-foreground">K/D Ratio</div>
              </div>
              <div className="text-center p-3 bg-muted rounded-lg">
                <div className="text-2xl font-bold text-primary">
                  {stats.lifetime.headshots}
                </div>
                <div className="text-sm text-muted-foreground">Headshots</div>
              </div>
            </div>
          </div>
        )}

        {/* Últimas Partidas */}
        {matches.length > 0 && (
          <div className="space-y-4">
            <h4 className="font-semibold flex items-center gap-2">
              <Calendar className="h-4 w-4" />
              Últimas Partidas
            </h4>
            <div className="space-y-2">
              {matches.slice(0, 3).map((match) => (
                <div key={match.matchId} className="flex items-center justify-between p-3 bg-muted rounded-lg">
                  <div className="flex items-center gap-3">
                    <div className="text-sm">
                      {formatDate(match.startedAt)}
                    </div>
                    <div className="text-sm text-muted-foreground">
                      {match.teams?.faction1?.players.length || 0} vs {match.teams?.faction2?.players.length || 0}
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <Badge variant={match.winner === 'faction1' ? 'default' : 'secondary'}>
                      {match.winner === 'faction1' ? 'Vitória' : 'Derrota'}
                    </Badge>
                    {match.results?.score && (
                      <span className="text-sm font-mono">
                        {match.results.score.faction1} - {match.results.score.faction2}
                      </span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Aviso sobre dados */}
        <div className="text-xs text-muted-foreground bg-muted p-3 rounded-lg">
          <p>💡 <strong>Dados da FACEIT:</strong> Estas informações são obtidas diretamente da FACEIT API e podem não estar sempre atualizadas.</p>
        </div>
      </CardContent>
    </Card>
  );
};

export default FaceitPlayerCard;
