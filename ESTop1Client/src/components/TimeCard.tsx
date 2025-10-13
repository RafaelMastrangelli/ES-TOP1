import { Link } from 'react-router-dom';
import { Time } from '@/types';
import { Card } from './ui/card';
import { Users } from 'lucide-react';
import { obterBandeiraPais } from '@/lib/utils';

interface TimeCardProps {
  time: Time;
}

const TimeCard = ({ time }: TimeCardProps) => {
  return (
    <Link to={`/times/${time.id}`}>
      <Card className="p-6 transition-all hover:shadow-lg hover:scale-[1.02] hover:border-primary">
        <div className="flex items-start justify-between">
          <div className="space-y-2">
            <h3 className="text-xl font-bold">{time.nome}</h3>
            <p className="text-sm text-muted-foreground">
              {obterBandeiraPais(time.pais)} {time.pais}
            </p>
          </div>
          <div className="flex items-center gap-2 text-muted-foreground">
            <Users className="h-5 w-5" />
            <span className="font-mono font-medium">
              {time.jogadores?.length || 0}
            </span>
          </div>
        </div>

        {time.jogadores && time.jogadores.length > 0 && (
          <div className="mt-4 pt-4 border-t">
            <p className="text-xs text-muted-foreground mb-2">Roster</p>
            <div className="flex flex-wrap gap-1">
              {time.jogadores.slice(0, 5).map((jogador) => (
                <span key={jogador.id} className="text-xs bg-secondary text-secondary-foreground px-2 py-1 rounded">
                  {jogador.apelido}
                </span>
              ))}
            </div>
          </div>
        )}
      </Card>
    </Link>
  );
};

export default TimeCard;
