import { Link } from 'react-router-dom';
import { Jogador } from '@/types';
import { Card } from './ui/card';
import { Badge } from './ui/badge';
import { TrendingUp, User } from 'lucide-react';
import { formatarMoeda, obterBandeiraPais } from '@/lib/utils';
import PlayerDefaultImage from './PlayerDefaultImage';

interface JogadorCardProps {
  jogador: Jogador;
}

const JogadorCard = ({ jogador }: JogadorCardProps) => {
  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Profissional':
        return 'bg-success text-white';
      case 'Aposentado':
        return 'bg-muted text-muted-foreground';
      case 'Amador':
        return 'bg-warning text-white';
      default:
        return 'bg-muted text-muted-foreground';
    }
  };

  const getDisponibilidadeColor = (disp: string) => {
    switch (disp) {
      case 'Livre':
        return 'bg-success text-white';
      case 'EmTime':
        return 'bg-secondary text-secondary-foreground';
      case 'Teste':
        return 'bg-warning text-white';
      default:
        return 'bg-muted text-muted-foreground';
    }
  };

  const getDisponibilidadeTexto = (disp: string) => {
    switch (disp) {
      case 'EmTime':
        return 'Em Time';
      case 'Livre':
        return 'Livre';
      case 'Teste':
        return 'Em Teste';
      default:
        return disp;
    }
  };

  return (
    <Link to={`/jogadores/${jogador.id}`}>
      <Card className="overflow-hidden transition-all hover:shadow-lg hover:scale-[1.02] hover:border-primary">
        <div className="aspect-square relative bg-gradient-dark">
          <PlayerDefaultImage 
            src={jogador.fotoUrl} 
            alt={jogador.apelido}
            className="w-full h-full object-cover opacity-80"
          />
          <div className="absolute top-2 right-2 flex gap-1">
            <Badge className={getStatusColor(jogador.status)}>
              {jogador.status}
            </Badge>
          </div>
          <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/80 to-transparent p-4">
            <h3 className="text-xl font-bold text-white">{jogador.apelido}</h3>
            <p className="text-sm text-white/80">
              {obterBandeiraPais(jogador.pais)} {jogador.idade} anos
            </p>
          </div>
        </div>

        <div className="p-4 space-y-3">
          <div className="flex items-center justify-between">
            <span className="text-sm text-muted-foreground">Time</span>
            <span className="text-sm font-medium">
              {jogador.timeAtual ? (
                <span className="flex items-center gap-1">
                  {obterBandeiraPais(jogador.timeAtual.pais)} {jogador.timeAtual.nome}
                </span>
              ) : (
                'Sem time'
              )}
            </span>
          </div>

          <div className="flex items-center justify-between">
            <span className="text-sm text-muted-foreground">Função</span>
            <Badge variant="secondary">{jogador.funcaoPrincipal}</Badge>
          </div>

          <div className="flex items-center justify-between">
            <span className="text-sm text-muted-foreground">Disponibilidade</span>
            <Badge className={getDisponibilidadeColor(jogador.disponibilidade)}>
              {getDisponibilidadeTexto(jogador.disponibilidade)}
            </Badge>
          </div>

          {jogador.ratingGeral && (
            <div className="flex items-center justify-between border-t pt-3">
              <div className="flex items-center gap-1">
                <TrendingUp className="h-4 w-4 text-primary" />
                <span className="text-sm font-medium">Rating</span>
              </div>
              <span className="text-lg font-bold font-mono text-primary">
                {jogador.ratingGeral.toFixed(2)}
              </span>
            </div>
          )}

          <div className="flex items-center justify-between border-t pt-3">
            <span className="text-sm text-muted-foreground">Valor de Mercado</span>
            <span className="text-sm font-bold text-success">
              {formatarMoeda(jogador.valorDeMercado)}
            </span>
          </div>
        </div>
      </Card>
    </Link>
  );
};

export default JogadorCard;
