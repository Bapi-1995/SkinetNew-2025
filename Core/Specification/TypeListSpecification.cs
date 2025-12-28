using System;
using Core.Entities;

namespace Core.Specification;

public class TypeListSpecification:BaseSpecification<Product,string>
{
public TypeListSpecification()
{
    AddSelect(p => p.Name);
    ApplyDistinct();
}

}
