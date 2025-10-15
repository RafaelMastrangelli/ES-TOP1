import { useState } from 'react';
import type { FiltrosTimes } from '@/types';
import { Input } from './ui/input';
import { Button } from './ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Card } from './ui/card';
import { Search, X } from 'lucide-react';

interface FiltrosTimesProps {
  filtros: FiltrosTimes;
  onChange: (filtros: FiltrosTimes) => void;
}

const FiltrosTimes = ({ filtros, onChange }: FiltrosTimesProps) => {
  const [buscaNome, setBuscaNome] = useState(filtros.nome || '');

  const handleBusca = () => {
    onChange({ ...filtros, nome: buscaNome, page: 1 });
  };

  const handleLimpar = () => {
    setBuscaNome('');
    onChange({ page: 1, pageSize: filtros.pageSize });
  };

  return (
    <Card className="p-4 space-y-4">
      {/* Busca por Nome */}
      <div className="space-y-3">
        <div className="flex gap-2">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Buscar por nome do time..."
              value={buscaNome}
              onChange={(e) => setBuscaNome(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleBusca()}
              className="pl-9"
            />
          </div>
          <Button 
            onClick={handleBusca}
            variant="outline"
            disabled={!buscaNome.trim()}
          >
            Buscar
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
        <Select
          value={filtros.tier || ''}
          onValueChange={(value) => onChange({ ...filtros, tier: value as any, page: 1 })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Tier" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="1">Tier 1</SelectItem>
            <SelectItem value="2">Tier 2</SelectItem>
            <SelectItem value="3">Tier 3</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={filtros.contratando || ''}
          onValueChange={(value) => onChange({ ...filtros, contratando: value as any, page: 1 })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Contratando" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="true">Sim</SelectItem>
            <SelectItem value="false">Não</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={filtros.ordenar || ''}
          onValueChange={(value) => onChange({ ...filtros, ordenar: value as any, page: 1 })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Ordenar por" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="nome_asc">Nome (A-Z)</SelectItem>
            <SelectItem value="tier_asc">Tier (Menor)</SelectItem>
            <SelectItem value="tier_desc">Tier (Maior)</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {(filtros.nome || filtros.tier || filtros.contratando || filtros.ordenar || buscaNome) && (
        <Button variant="outline" onClick={handleLimpar} className="w-full">
          <X className="h-4 w-4 mr-2" />
          Limpar Filtros
        </Button>
      )}
    </Card>
  );
};

export default FiltrosTimes;
