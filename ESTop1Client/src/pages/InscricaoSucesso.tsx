import { useLocation, Link } from 'react-router-dom';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { CheckCircle, Target, ArrowRight } from 'lucide-react';
import Navbar from '@/components/Navbar';

const InscricaoSucesso = () => {
  const location = useLocation();
  const inscricaoId = location.state?.inscricaoId;

  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      
      <div className="container mx-auto px-4 py-16">
        <div className="max-w-2xl mx-auto">
          <Card className="border-border bg-card animate-fade-in">
            <CardHeader className="text-center space-y-4">
              <div className="flex justify-center">
                <div className="p-4 rounded-full bg-success/10">
                  <CheckCircle className="h-12 w-12 text-success" />
                </div>
              </div>
              <div>
                <CardTitle className="text-3xl font-bold text-success">Inscrição Realizada!</CardTitle>
                <CardDescription className="text-lg">
                  Sua inscrição foi enviada com sucesso
                </CardDescription>
              </div>
            </CardHeader>
            
            <CardContent className="space-y-6">
              <div className="text-center space-y-4">
                <p className="text-muted-foreground">
                  Obrigado por se inscrever na ESTop1! Sua inscrição está sendo analisada pela nossa equipe.
                </p>
                
                {inscricaoId && (
                  <div className="p-4 bg-muted/50 rounded-lg">
                    <p className="text-sm font-mono text-muted-foreground">
                      ID da Inscrição: {inscricaoId}
                    </p>
                  </div>
                )}
              </div>

              <div className="space-y-4">
                <h3 className="text-lg font-semibold">Próximos passos:</h3>
                <div className="space-y-3">
                  <div className="flex items-start space-x-3">
                    <div className="p-1 rounded-full bg-primary/10 mt-1">
                      <Target className="h-4 w-4 text-primary" />
                    </div>
                    <div>
                      <p className="font-medium">Análise do perfil</p>
                      <p className="text-sm text-muted-foreground">
                        Nossa equipe analisará seu perfil e estatísticas
                      </p>
                    </div>
                  </div>
                  
                  <div className="flex items-start space-x-3">
                    <div className="p-1 rounded-full bg-secondary/10 mt-1">
                      <CheckCircle className="h-4 w-4 text-secondary" />
                    </div>
                    <div>
                      <p className="font-medium">Aprovação</p>
                      <p className="text-sm text-muted-foreground">
                        Se aprovado, seu perfil ficará visível para times
                      </p>
                    </div>
                  </div>
                  
                  <div className="flex items-start space-x-3">
                    <div className="p-1 rounded-full bg-accent/10 mt-1">
                      <ArrowRight className="h-4 w-4 text-accent" />
                    </div>
                    <div>
                      <p className="font-medium">Oportunidades</p>
                      <p className="text-sm text-muted-foreground">
                        Times poderão entrar em contato com você
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              <div className="pt-6 border-t space-y-4">
                <p className="text-sm text-muted-foreground text-center">
                  Você receberá um email de confirmação em breve. 
                  O processo de análise pode levar de 1 a 3 dias úteis.
                </p>
                
                <div className="flex flex-col sm:flex-row gap-3">
                  <Button asChild className="flex-1">
                    <Link to="/jogadores">
                      Explorar Jogadores
                      <ArrowRight className="ml-2 h-4 w-4" />
                    </Link>
                  </Button>
                  <Button variant="outline" asChild className="flex-1">
                    <Link to="/">Voltar ao Início</Link>
                  </Button>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default InscricaoSucesso;
