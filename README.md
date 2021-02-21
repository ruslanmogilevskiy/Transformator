# Transformator
Transforms one data object into one or many of others with flexibly and fluently defined tree of transformers.

**Downloads**: 

![nuget](https://img.shields.io/nuget/v/Rumo.Transformator)


**Use cases**: 
* you have an income message/event/request and based on it you need to produce one or more, possibly linked to or dependant on each other data objects. This could be done in scope of a business workflow or an income request processing.

**Features**

* 1-to-1 or 1-to-many transformation results are supported. I.e. single source data object could produce many destination (results) data objects with a single transformation.
* Data transformers could have linear formation or as a tree, that allows to flexibly build the transformation logic. Other transformators could be used as a transformation tree's leaf.
* Data transformer could return multiple destination results that forms separate transformation results and all the following transformers will be applier to.
* Transformers could produce an isolated result, that allows to produce multiple transformation results from a single entry data object.
* Fluent building of the transformation tree.

# TODOs
* documentation + example