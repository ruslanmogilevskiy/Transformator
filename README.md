# Transformator
Transforms one source data object to one or many destination objects with flexibly and fluently defined tree of transformers, with a single responsibility, testability, and reusability in mind from the beginning.

**Packages**

<a href="https://www.nuget.org/packages/Rumo.Transformator/" alt="Contributors">
    <img src="https://img.shields.io/nuget/v/Rumo.Transformator" /><br/><br/>
</a>



**The ideas behind the Transformator**

* Transformator requires that you have a source data object to transform. 
* Then you split your data transformation flow to steps, they will be transformers. There could be one or many transformers involved to produce one _destination_ object (transformation result).
* Each transformer uses the previous transformer's result as a basis to do own transformation but could create an other destination object or even of a different type.
* By default, all transformers are grouped into one group and produce a single destination object.
* But some transformers could run own isolated (from the main transformation flow) transformation by forming a transformation tree's leaf. In this case, they produce a separate destination object, i.e. the whole transformation flow produces multiple transformation results.

![alt text](docs/img/transformation_flow.png?raw=true)


**Use case**

Say you've received a 'new order' event and based on you need to produce one or more, possibly linked or dependant on each other notification messages for the customer, like an email, Telegram and Slack messages, and also format a log message based on the sent email to be stored in your system's log, as described on the below picture:

![alt text](docs/img/use_case.png?raw=true)




**Features**

* 1-to-1 or 1-to-many transformation results are supported. I.e. single source data object could produce many destination (results) data objects with a single transformation.
* Data transformers could have linear formation or as a tree, that allows to flexibly build the transformation logic. Other transformators could be used as a transformation tree's leaf.
* Data transformer could return multiple destination results that forms separate transformation results and all the following transformers will be applier to.
* Transformers could produce an isolated result, that allows to produce multiple transformation results from a single entry data object.
* Fluent building of the transformation tree.

# TODOs
* documentation + example