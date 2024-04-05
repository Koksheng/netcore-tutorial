# Caching

A process of storing frequently accessed data in a temporary storage location.

To accelerate data delivery to clients, as it eliminates the need to repeatedly fetch the same data from the original source.

## Cache Control

Cache-control is an HTTP header used to specfiy browser caching policies in both client requests and server responses. 

Policies include how a resource is cached, where it's cached and its maximum age before expiring.

![abc](https://www.imperva.com/learn/wp-content/uploads/sites/13/2019/01/response-headers.jpg.webp)

## Cache Control: Max-Age

cache-control: max-age=120 means that the returned resource is valid for 120 seconds, afther which the browser has to request a newer version.


[Link to Reference Cache](https://www.imperva.com/learn/performance/cache-control/)