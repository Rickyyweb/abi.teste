using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

// Desabilita a paralelização de classes na assembly de testes de integração
[assembly: CollectionBehavior(DisableTestParallelization = true)]

