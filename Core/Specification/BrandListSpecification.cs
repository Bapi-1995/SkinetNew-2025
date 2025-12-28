using System;
using Core.Entities;

namespace Core.Specification;

public class BrandListSpecification: BaseSpecification<Product,string>
{
    public BrandListSpecification()
    {
        AddSelect(p => p.Name);
        ApplyDistinct();
        
    }

}
